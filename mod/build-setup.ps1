param(
  [switch]$Clean
)

$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $root

Write-Host "[1/4] Prüfe npm..."
npm --version | Out-Null

if ($Clean) {
  Write-Host "[2/4] Clean: node_modules + package-lock.json entfernen..."
  if (Test-Path "$root\node_modules") { Remove-Item "$root\node_modules" -Recurse -Force }
  if (Test-Path "$root\package-lock.json") { Remove-Item "$root\package-lock.json" -Force }
}

Write-Host "[3/4] Dependencies installieren..."
npm install

Write-Host "[4/4] NSIS-Setup bauen..."
npm run dist

$setup = Get-ChildItem "$root\dist\*Setup-*.exe" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($setup) {
  Write-Host "\nFERTIG ✅"
  Write-Host "Setup: $($setup.FullName)"
} else {
  Write-Host "\nBuild lief durch, aber Setup-Datei nicht gefunden. Prüfe dist\\"
}
