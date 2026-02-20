@echo off
setlocal
cd /d "%~dp0"

echo [1/3] Dependencies installieren...
call npm install
if errorlevel 1 goto :err

echo [2/3] NSIS-Setup bauen...
call npm run dist
if errorlevel 1 goto :err

echo [3/3] Fertig.
echo Setup liegt in: %cd%\dist
exit /b 0

:err
echo.
echo Build fehlgeschlagen.
exit /b 1
