Write-Output "Started building/publishing"

# GTiff2Tiles.Core -- build and create nuget package

Write-Output "Start building GTiff2Tiles.Core"
dotnet build "GTiff2Tiles.Core/GTiff2Tiles.Core.csproj" -c Release /p:Platform=x64
Write-Output "Ended building GTiff2Tiles.Core"

# GTiff2Tiles.Benhmarks

# Win-x64
Write-Output "Start Win-x64-Benchmarks publish"
dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Benchmarks/win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Win-x64-Benchmarks publish"

# Linux-x64
Write-Output "Start Linux-x64-Benchmarks publish"
dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Benchmarks/linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Linux-x64-Benchmarks publish"

# GTiff2Tiles.Console

# Win-x64
Write-Output "Start Win-x64-Console publish"
dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Console/win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Win-x64-Console publish"

# Linux-x64
Write-Output "Start Linux-x64-Console publish"
dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Console/linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
Write-Output "Ended Linux-x64-Console publish"

# GTiff2Tiles.GUI
if ($IsWindows)
{
    # Win-x64
    Write-Output "Start Win-x64-GUI publish"
    dotnet publish "GTiff2Tiles.GUI/GTiff2Tiles.GUI.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.GUI/win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
    Write-Output "Ended Win-x64-GUI publish"
}

Write-Output "Ended building/publishing"

# Remove all *.pdb files
Write-Output "Removing all *.pdb files from Publish directory"
Get-ChildItem "Publish/" -Include *.pdb -Recurse | Remove-Item

# TODO: add documentation file, etc to published directories before zipping them

# Add everything into archives. Requires installed 7z added to PATH.

# Placeholder - 7z add win-x64-GUI to .zip
# Start-Process -NoNewWindow -FilePath "7z" -ArgumentList "a ../../Win-x64-GTiff2Tiles.GUI.zip *" -WorkingDirectory "Publish/GTiff2Tiles.GUI/win-x64/"

$7zStartInfo = New-Object System.Diagnostics.ProcessStartInfo
$7zStartInfo.FileName = "7z"
$7zStartInfo.CreateNoWindow = $true
$process = New-Object System.Diagnostics.Process
$process.StartInfo = $7zStartInfo

Write-Output "Started adding published files to .zip archives"

# Add Win-x64-GUI to .zip
if ($IsWindows)
{
    Write-Output "Start adding Win-x64-GUI to .zip"
    $7zStartInfo.Arguments = "a ../../Win-x64-GTiff2Tiles.GUI.zip *"
    $7zStartInfo.WorkingDirectory = "Publish/GTiff2Tiles.GUI/win-x64/"
    $process.Start() # | Out-Null
    $process.WaitForExit()
    Write-Output "Ended adding Win-x64-GUI to .zip"
}

# Add Win-x64-Console to .zip
Write-Output "Start adding Win-x64-Console to .zip"
$7zStartInfo.Arguments = "a ../../Win-x64-GTiff2Tiles.Console.zip *"
$7zStartInfo.WorkingDirectory = "Publish/GTiff2Tiles.Console/win-x64/"
$process.Start() # | Out-Null
$process.WaitForExit()
Write-Output "Ended adding Win-x64-Console to .zip"

# Add Linux-x64-Console to .zip
Write-Output "Start adding Linux-x64-Console to .zip"
$7zStartInfo.Arguments = "a ../../Linux-x64-GTiff2Tiles.Console.zip *"
$7zStartInfo.WorkingDirectory = "Publish/GTiff2Tiles.Console/linux-x64/"
$process.Start() # | Out-Null
$process.WaitForExit()
Write-Output "Ended adding Linux-x64-Console to .zip"

Write-Output "Ended adding published files to .zip archives"