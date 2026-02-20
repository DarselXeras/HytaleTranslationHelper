param(
  [string]$Version = "0.2.0"
)

$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$sourceDir = Join-Path $root "dist\win-unpacked"
$appDir = Join-Path $sourceDir "Hytale Languagefile Editor"
$releaseDir = Join-Path $root "release"

if (!(Test-Path $appDir)) {
  throw "Source folder not found: $appDir`nBuild first: npm run dist"
}

New-Item -ItemType Directory -Force -Path $releaseDir | Out-Null

$zipName = "Hytale-Languagefile-Editor-v$Version.zip"
$zipPath = Join-Path $releaseDir $zipName
if (Test-Path $zipPath) { Remove-Item $zipPath -Force }

$tempStage = Join-Path $releaseDir "_stage"
if (Test-Path $tempStage) { Remove-Item $tempStage -Recurse -Force }
New-Item -ItemType Directory -Force -Path $tempStage | Out-Null

$stageAppDir = Join-Path $tempStage "Hytale Languagefile Editor"
Copy-Item $appDir $stageAppDir -Recurse -Force

# Optional docs (if present)
$manual = Join-Path $root "USER_MANUAL_EN.md"
if (Test-Path $manual) {
  Copy-Item $manual (Join-Path $stageAppDir "USER_MANUAL_EN.md") -Force
}

# Remove obvious non-release clutter if it slipped in
$removePatterns = @(
  "*.pdb",
  "*.xml",
  "*.log"
)
foreach ($pat in $removePatterns) {
  Get-ChildItem -Path $stageAppDir -Recurse -File -Filter $pat -ErrorAction SilentlyContinue | Remove-Item -Force -ErrorAction SilentlyContinue
}

Compress-Archive -Path (Join-Path $tempStage "*") -DestinationPath $zipPath -CompressionLevel Optimal -Force

$hash = (Get-FileHash -Path $zipPath -Algorithm SHA256).Hash

Remove-Item $tempStage -Recurse -Force

Write-Host ""
Write-Host "Done ✅"
Write-Host "ZIP: $zipPath"
Write-Host "SHA256: $hash"
