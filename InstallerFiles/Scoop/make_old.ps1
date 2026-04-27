$file1 = "add_to_openwith_menu.ps1"
$file2 = "clean_registry.reg"
$file3 = "subload.exe"
$file4 = "subloader-cli.exe"

$directoryPath = "Output"

if (!(Test-Path -Path $directoryPath -PathType Container)) {
    New-Item -Path $directoryPath -ItemType Directory
}

$outputZip = "Output/scoop.zip"

Write-Host "Creating archive..." -ForegroundColor Cyan
Compress-Archive -Path $file1, $file2, $file3, $file4 -DestinationPath $outputZip -Force

Write-Host "Calculating hash..." -ForegroundColor Cyan
$hash = Get-FileHash -Algorithm SHA256 -Path $outputZip

Set-Content -Path ($outputZip + ".sha256") -Value $hash.Hash.ToLowerInvariant() -NoNewline
Write-Host "DONE" -ForegroundColor Green