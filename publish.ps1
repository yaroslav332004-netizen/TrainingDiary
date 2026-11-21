$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

foreach ($rid in @("win-x64", "osx-arm64", "linux-x64")) {
    Write-Host "== $rid"
    dotnet publish TrainingDiary.App -c Release -r $rid `
        --self-contained true -o "publish/$rid"
}
