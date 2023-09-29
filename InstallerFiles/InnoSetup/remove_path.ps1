param([string]$pathToRemove)

$currentPath = [System.Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::Machine)
$pathList = $currentPath -split ";"
$pathList = $pathList | Where-Object { $_ -ne $pathToRemove }
$newPath = $pathList -join ";"
[System.Environment]::SetEnvironmentVariable("PATH", $newPath, [System.EnvironmentVariableTarget]::Machine)