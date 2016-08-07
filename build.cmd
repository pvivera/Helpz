rem @echo off

".paket\paket.bootstrapper.exe" restore

"packages\Build\FAKE.Core\tools\Fake.exe" "build.fsx" "nugetApikey=%NUGET_APIKEY%" "buildVersion=%APPVEYOR_BUILD_VERSION%"

exit /b %errorlevel%