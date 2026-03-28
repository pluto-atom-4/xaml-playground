@echo off
REM Scientific App Launcher
REM Run the standalone Scientific App (no .NET installation required)

cd /d "%~dp0"

set STANDALONE_EXE=ScientificApp\bin\Release\net8.0-windows\win-x64\publish\ScientificApp.exe

if exist "%STANDALONE_EXE%" (
    echo Launching standalone executable...
    "%STANDALONE_EXE%"
) else (
    echo Standalone executable not found. Building...
    dotnet publish -c Release -r win-x64 --self-contained
    if exist "%STANDALONE_EXE%" (
        echo Build successful. Launching...
        "%STANDALONE_EXE%"
    ) else (
        echo Build failed. Falling back to debug mode...
        dotnet run --project ScientificApp/ScientificApp.csproj
    )
)

pause
