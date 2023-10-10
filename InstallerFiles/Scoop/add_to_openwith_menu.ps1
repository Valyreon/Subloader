function WriteRegistry {
    param (
        [string]$Path,
        [string]$Name,
        [string]$Value
    )

    $keyExists = Test-Path -Path $Path
    if(!$keyExists) {
        New-Item -Path $Path -Force | Out-Null
    }

    # Set the registry value with the provided key value
    Set-ItemProperty -Path $Path -Name $Name -Value $Value | Out-Null
}

$extensions = @(".mp4", ".mkv", ".avi")

foreach ($ext in $extensions) {
    $registryPath = "HKCU:\Software\Classes\Subloader" + $ext + "\shell\open"

    WriteRegistry -Path $registryPath -Name "FriendlyAppName" -Value "Subloader"

    $registryPath += "\command"
    WriteRegistry -Path $registryPath -Name "(Default)" -Value ("`"" + $PSScriptRoot + "\subload.exe" + "`" `"%1`"")

    $openWithRegPath = "HKCU:\Software\Classes\" + $ext + "\OpenWithProgIds"
    WriteRegistry -Path $openWithRegPath -Name ("Subloader" + $ext) -Value ""

    Write-Host ("Added entry to 'Open with' menu for " + $ext + " files.") -ForegroundColor Cyan
}