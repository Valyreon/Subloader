param (
    [string]$Version
)

$versionRegex = "^\d\.\d\.\d$"
if(-not $Version -or (-not ($Version -match $versionRegex))) {
    Write-Host "Please input the version argument in the correct format: a.b.c" -ForegroundColor Red
    Exit 0
}

$versionWithoutDots = $Version -replace "\.", ""

$outputPath = "./ReleaseOutput"
$innoFolder = "./ReleaseOutput/Inno"
$scoopOutputPath = "./ReleaseOutput/Scoop"
$portableOutputPath = "./ReleaseOutput/Portable"
$ErrorActionPreference = "Stop"

# clean the output folder
Write-Host "Cleaning output folder..." -ForegroundColor Cyan
if (Test-Path -Path $outputPath -PathType Container) {
    Remove-Item -Path $outputPath -Recurse -Force
}

$publishWpfReleaseCommand = "dotnet publish SubloaderWpf -c 'Release' -p:PublishSingleFile=true --no-self-contained -r win-x64 -o " + $outputPath
$publishWpfPortableCommand = "dotnet publish SubloaderWpf -c 'Portable Release' -p:PublishSingleFile=true --no-self-contained -r win-x64 -o " + $portableOutputPath
$publishWpfScoopCommand = "dotnet publish SubloaderWpf -c 'Scoop Release' -p:PublishSingleFile=true --no-self-contained -r win-x64 -o " + $scoopOutputPath
$publishCliCommand = "dotnet publish SubloaderCLI -c 'Release' -p:PublishSingleFile=true --no-self-contained -r win-x64 -o " + $outputPath

Write-Host "Publishing regular release..." -ForegroundColor Cyan
Invoke-Expression $publishWpfReleaseCommand 2>&1 | Out-Null

Write-Host "Publishing portable release..." -ForegroundColor Cyan
Invoke-Expression $publishWpfPortableCommand 2>&1 | Out-Null

Write-Host "Publishing scoop release..." -ForegroundColor Cyan
Invoke-Expression $publishWpfScoopCommand 2>&1 | Out-Null

Write-Host "Publishing CLI release..." -ForegroundColor Cyan
Invoke-Expression $publishCliCommand 2>&1 | Out-Null

# Scoop archive and hash create
Write-Host "Creating scoop archive and hash..." -ForegroundColor Cyan

Rename-Item -Path ($scoopOutputPath + "/SubloaderWpf.exe") -NewName "subload.exe"
Rename-Item -Path ($outputPath + "/SubloaderCLI.exe") -NewName "subloader-cli.exe"

$file1 = "InstallerFiles/Scoop/add_to_openwith_menu.ps1"
$file2 = "InstallerFiles/Scoop/clean_registry.reg"
$file3 = $scoopOutputPath + "/subload.exe"
$file4 = $outputPath + "/subloader-cli.exe"

$outputZip = $outputPath + "/scoop.zip"

Write-Host "Creating archive..." -ForegroundColor Cyan
Compress-Archive -Path $file1, $file2, $file3, $file4 -DestinationPath $outputZip -Force

Write-Host "Calculating hash..." -ForegroundColor Cyan
$hash = Get-FileHash -Algorithm SHA256 -Path $outputZip

Set-Content -Path ($outputZip + ".sha256") -Value $hash.Hash.ToLowerInvariant() -NoNewline
Remove-Item -Path $scoopOutputPath -Recurse -Force

$scoopManifestFile = "InstallerFiles/Scoop/subloader.json"
$scoopManifest = Get-Content -Path $scoopManifestFile
$hashLineRegexPattern = "`"hash`"\s*:\s*`"[a-z0-9]{64}`""
$versionLineRegexPattern = "`"version`"\s*:\s*`"\d\.\d\.\d`""
$scoopManifest = $scoopManifest -replace $hashLineRegexPattern, ("`"hash`": `"" + $hash.Hash.ToLowerInvariant() + "`"")
$scoopManifest = $scoopManifest -replace $versionLineRegexPattern, ("`"version`": `"" + $Version + "`"")
Set-Content -Path $scoopManifestFile -Value $scoopManifest

# Create Inno setup
Write-Host "Creating installer..." -ForegroundColor Cyan
$innoFilesToCopy = @(
    "InstallerFiles/InnoSetup/icon.ico",
    "InstallerFiles/InnoSetup/inno_setup.iss",
    "InstallerFiles/InnoSetup/licence.txt",
    "InstallerFiles/InnoSetup/remove_path.ps1",
    "InstallerFiles/InnoSetup/set_path.ps1")
New-Item -Path $innoFolder -ItemType Directory | Out-Null

foreach ($file in $innoFilesToCopy) {
    $fileName = Split-Path -Path $file -Leaf
    Copy-Item -Path $file -Destination ($innoFolder + "/" + $fileName)
}

Copy-Item -Path ($outputPath + "/SubloaderWpf.exe") -Destination ($innoFolder + "/SubLoad.exe")
Copy-Item -Path ($outputPath + "/subloader-cli.exe") -Destination ($innoFolder + "/subloader-cli.exe")

$compileInnoSetupExpression = "& 'C:/Program Files (x86)/Inno Setup 6/ISCC.exe' /q " + $innoFolder + "/inno_setup.iss"
Invoke-Expression $compileInnoSetupExpression 2>&1

$setupFile = (Get-ChildItem -Path ($innoFolder + "/Output") -File)[0]
Copy-Item -Path $setupFile -Destination ($outputPath + "/SubloaderV" + $versionWithoutDots + "Installer.exe")
Remove-Item -Path $innoFolder -Recurse -Force

# Copy Portable file
Write-Host "Cleaning up..." -ForegroundColor Cyan
Copy-Item -Path ($portableOutputPath + "/SubloaderWpf.exe") -Destination ($outputPath + "/SubloaderV" + $versionWithoutDots + "Portable.exe")
Remove-Item -Path $portableOutputPath -Recurse -Force
Remove-Item -Path ($outputPath + "/SubloaderWpf.exe")

Rename-Item -Path ($outputPath + "/subloader-cli.exe") -NewName ("subloader-cli-v" + $versionWithoutDots + ".exe")

Write-Host "DONE" -ForegroundColor Green
Write-Host ""