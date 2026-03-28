# Scientific App Launcher (PowerShell)
# This script runs the standalone Scientific App (no .NET installation required)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$standaloneExe = Join-Path $scriptDir "ScientificApp\bin\Release\net8.0-windows\win-x64\publish\ScientificApp.exe"

if (Test-Path $standaloneExe) {
    Write-Host "Launching standalone executable..." -ForegroundColor Green
    & $standaloneExe
} else {
    Write-Host "Standalone executable not found. Building..." -ForegroundColor Yellow
    Set-Location $scriptDir
    dotnet publish -c Release -r win-x64 --self-contained
    
    if (Test-Path $standaloneExe) {
        Write-Host "Build successful. Launching..." -ForegroundColor Green
        & $standaloneExe
    } else {
        Write-Host "Build failed. Falling back to debug mode..." -ForegroundColor Red
        dotnet run --project ScientificApp/ScientificApp.csproj
    }
}
