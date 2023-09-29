# Define the path you want to add
param([string]$newPath)

# Get the current value of the PATH environment variable
$currentPath = [System.Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::Machine)

# Check if the path already exists in the current PATH variable
if ($currentPath -split ";" -notcontains $newPath) {
    # Add the new path to the PATH variable
    [System.Environment]::SetEnvironmentVariable("PATH", "$currentPath;$newPath", [System.EnvironmentVariableTarget]::Machine)

    # Inform the user
    Write-Host "Path added to the PATH environment variable. Changes will take effect after you restart your shell."
} else {
    Write-Host "The path already exists in the PATH environment variable."
}