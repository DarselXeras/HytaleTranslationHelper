@echo off
setlocal
cd /d "%~dp0"

if "%~1"=="" (
  powershell -ExecutionPolicy Bypass -File "%~dp0package-release-curseforge.ps1" -Version 0.2.0
) else (
  powershell -ExecutionPolicy Bypass -File "%~dp0package-release-curseforge.ps1" -Version %1
)

if errorlevel 1 (
  echo.
  echo Packaging failed.
  exit /b 1
)

echo.
echo Packaging finished.
exit /b 0
