#r @"packages\Build\FAKE.Core\tools\FakeLib.dll"
open System
open Fake 
open Fake.AssemblyInfoFile

let releaseNotes = 
    ReadFile "RELEASE_NOTES.md"
    |> ReleaseNotesHelper.parseReleaseNotes

let buildMode = getBuildParamOrDefault "buildMode" "Release"
let buildVersion = getBuildParamOrDefault "buildVersion" "0.0.1"
let nugetApikey = getBuildParamOrDefault "nugetApikey" ""

let dirPackages = "./Build/Packages"
let dirReports = "./Build/Reports"
let filePathUnitTestReport = dirReports + "/NUnit.xml"
let fileListUnitTests = !! ("**/bin/" @@ buildMode @@ "/Helpz*Tests.dll")
let toolNUnit = "./packages/build/NUnit.Runners/tools"
let toolIlMerge = "./packages/Build/ilmerge/tools/ILMerge.exe"
let nugetVersion = buildVersion // + "-alpha"
let nugetVersionDep = "["+nugetVersion+"]"


Target "Clean" (fun _ ->
    CleanDirs [ dirPackages; dirReports ]
    )

Target "SetVersion" (fun _ ->
    CreateCSharpAssemblyInfo "./Source/SolutionInfo.cs"
        [Attribute.Version buildVersion
         Attribute.InformationalVersion nugetVersion
         Attribute.FileVersion buildVersion]
    )

Target "BuildApp" (fun _ ->
    MSBuild null "Build" ["Configuration", buildMode] ["./Helpz.sln"]
    |> Log "AppBuild-Output: "
    )

Target "UnitTest" (fun _ ->
    fileListUnitTests
        |> NUnit (fun p -> 
            {p with
                DisableShadowCopy = true;
                Framework = "net-4.0";
                ToolPath = toolNUnit;
                TimeOut = TimeSpan.FromMinutes 30.0;
                ToolName = "nunit-console-x86.exe";
                OutputFile = filePathUnitTestReport})
    )

Target "CreatePackageHelpz" (fun _ ->
    let binDir = "Source/Helpz/bin/"
    CopyFile binDir (binDir + buildMode + "/Helpz.dll")
    NuGet (fun p ->
        {p with
            OutputPath = dirPackages
            WorkingDir = "Source/Helpz"
            Version = nugetVersion
            ReleaseNotes = toLines releaseNotes.Notes
            Publish = false })
            "Source/Helpz/Helpz.nuspec"
    )

Target "CreatePackageHelpzHttpMock" (fun _ ->
    let binDir = "Source\\Helpz.HttpMock\\bin\\" + buildMode + "\\"
    let result = ExecProcess (fun info ->
       info.Arguments <- "/targetplatform:v4 /internalize /allowDup /target:library /out:Source\\Helpz.HttpMock\\bin\\Helpz.HttpMock.dll " + binDir + "Helpz.HttpMock.dll " + binDir + "Microsoft.Owin.Host.HttpListener.dll " + binDir + "Microsoft.Owin.Hosting.dll"
       info.FileName <- toolIlMerge) (TimeSpan.FromMinutes 5.0)
    if result <> 0 then failwithf "ILMerge of Helpz.HttpMock returned with a non-zero exit code"
    NuGet (fun p -> 
        {p with
            OutputPath = dirPackages
            WorkingDir = "Source/Helpz.HttpMock"
            Version = nugetVersion
            ReleaseNotes = toLines releaseNotes.Notes
            Dependencies = [
                "Owin",  GetPackageVersion "./packages/" "Owin"
                "Helpz", nugetVersionDep
                "Microsoft.Owin",  GetPackageVersion "./packages/" "Microsoft.Owin"
                ]
            Publish = false })
            "Source/Helpz.HttpMock/Helpz.HttpMock.nuspec"
    )

Target "CreatePackageHelpzSQLite" (fun _ ->
    let binDir = "Source/Helpz.SQLite/bin/"
    CopyFile binDir (binDir + buildMode + "/Helpz.SQLite.dll")
    NuGet (fun p ->
        {p with
            OutputPath = dirPackages
            WorkingDir = "Source/Helpz.SQLite"
            Version = nugetVersion
            ReleaseNotes = toLines releaseNotes.Notes
            Dependencies = [
                "System.Data.SQLite.Core",  GetPackageVersion "./packages/" "System.Data.SQLite.Core"
            ]            
            Publish = false })
            "Source/Helpz.SQLite/Helpz.SQLite.nuspec"
    )

Target "Default" DoNothing

"Clean"
    ==> "SetVersion"
    ==> "BuildApp"
    ==> "UnitTest"
    ==> "CreatePackageHelpz"
    ==> "CreatePackageHelpzHttpMock"
    ==> "CreatePackageHelpzSQLite"
    ==> "Default"

RunTargetOrDefault "Default"
