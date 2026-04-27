if ($global) {
    $rootRegPath = "HKLM:"
} else {
    $rootRegPath = "HKCU:"
}

$extensions = @("mp4", "mkv", "avi")

foreach ($ext in $extensions) {
    # Remove ProgId keys
    $progIdPath = "$rootRegPath\SOFTWARE\Classes\Subloader.$ext"
    if (Test-Path $progIdPath) {
        Remove-Item -Path $progIdPath -Recurse -Force
    }

    # Remove shell context menu keys
    $shellPath = "$rootRegPath\Software\Classes\SystemFileAssociations\.$ext\shell\Subloader"
    if (Test-Path $shellPath) {
        Remove-Item -Path $shellPath -Recurse -Force
    }

    # Remove OpenWithProgIds value
    $openWithPath = "$rootRegPath\SOFTWARE\Classes\.$ext\OpenWithProgIds"
    if (Test-Path $openWithPath) {
        Remove-ItemProperty -Path $openWithPath -Name "Subloader.$ext" -Force -ErrorAction SilentlyContinue
    }
}
