$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

foreach ($rid in @("win-x64", "osx-arm64", "linux-x64")) {
    Write-Host "== $rid"
    foreach ($project in @("TrainingDiary.App", "TrainingDiary.Desktop")) {
        dotnet publish $project -c Release -r $rid `
            --self-contained true -o "publish/$rid/$project"
    }
}
