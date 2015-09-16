#r @"tools\FAKE.Core\tools\FakeLib.dll"
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
let toolNUnit = "./Tools/NUnit.Runners/tools"
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
                ToolPath = "./Tools/NUnit.Runners/tools";
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

Target "Default" DoNothing

"Clean"
    ==> "SetVersion"
    ==> "BuildApp"
    ==> "UnitTest"
    ==> "CreatePackageHelpz"
    ==> "Default"

RunTargetOrDefault "Default"
