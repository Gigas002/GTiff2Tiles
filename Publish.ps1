#TODO: Test script on Linux

Write-Output "Started building/publishing"

# GTiff2Tiles.Core -- build and create nuget package

Write-Output "Start building GTiff2Tiles.Core"
dotnet build "GTiff2Tiles.Core/GTiff2Tiles.Core.csproj" -c Release /p:Platform=x64
Write-Output "Ended building GTiff2Tiles.Core"

# GTiff2Tiles.Benhmarks

# Win-x64
Write-Output "Start Win-x64-Benchmarks publish"
if ($IsWindows)
{
    dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Benchmarks/win-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:PublishReadyToRun=true
}
else
{
    dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Benchmarks/win-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true
}
Write-Output "Ended Win-x64-Benchmarks publish"

# Linux-x64
Write-Output "Start Linux-x64-Benchmarks publish"
if ($IsWindows)
{
    dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Benchmarks/linux-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true
}
else
{
    dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Benchmarks/linux-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:PublishReadyToRun=true
}
Write-Output "Ended Linux-x64-Benchmarks publish"

# GTiff2Tiles.Console

# Win-x64
Write-Output "Start Win-x64-Console publish"
if ($IsWindows)
{
    dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Console/win-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:PublishReadyToRun=true
}
else
{
    dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Console/win-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true
}
Write-Output "Ended Win-x64-Console publish"

# Linux-x64
Write-Output "Start Linux-x64-Console publish"
if ($IsWindows)
{
    dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Console/linux-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true
}
else
{
    dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Console/linux-x64 /p:IncludeAllContentInSingleFile=true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:PublishReadyToRun=true
}
Write-Output "Ended Linux-x64-Console publish"

# GTiff2Tiles.GUI
if ($IsWindows)
{
    # Win-x64
    Write-Output "Start Win-x64-GUI publish"
    dotnet publish "GTiff2Tiles.GUI/GTiff2Tiles.GUI.csproj" -c Release -o Publish/GTiff2Tiles.GUI/win-x64
    Write-Output "Ended Win-x64-GUI publish"
}

Write-Output "Ended building/publishing"

# Remove all *.pdb files
Write-Output "Removing all *.pdb files from Publish directory"
Get-ChildItem "Publish/" -Include *.pdb -Recurse | Remove-Item

# Copy docs, etc to published directories before zipping them
Write-Output "Copying docs, LICENSE.md, etc to published directories"

Copy-Item -Path "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.Doc.pdf" -Destination "Publish/GTiff2Tiles.Benchmarks/win-x64/GTiff2Tiles.Benchmarks.Doc.pdf"
Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.Benchmarks/win-x64/LICENSE.md"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.Benchmarks/win-x64/CHANGELOG.md"

Copy-Item -Path "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.Doc.pdf" -Destination "Publish/GTiff2Tiles.Benchmarks/linux-x64/GTiff2Tiles.Benchmarks.Doc.pdf"
Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.Benchmarks/linux-x64/LICENSE.md"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.Benchmarks/linux-x64/CHANGELOG.md"

Copy-Item -Path "GTiff2Tiles.Console/GTiff2Tiles.Console.Doc.pdf" -Destination "Publish/GTiff2Tiles.Console/win-x64/GTiff2Tiles.Console.Doc.pdf"
Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.Console/win-x64/LICENSE.md"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.Console/win-x64/CHANGELOG.md"

Copy-Item -Path "GTiff2Tiles.Console/GTiff2Tiles.Console.Doc.pdf" -Destination "Publish/GTiff2Tiles.Console/linux-x64/GTiff2Tiles.Console.Doc.pdf"
Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.Console/linux-x64/LICENSE.md"
Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.Console/linux-x64/CHANGELOG.md"

if ($IsWindows)
{
    Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.GUI/win-x64/LICENSE.md"
    Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.GUI/win-x64/CHANGELOG.md"
}

# Add everything into archives. Requires installed 7z added to PATH.

Write-Output "Started adding published files to .zip archives"

# Add Win-x64-GUI to .zip
if ($IsWindows)
{
    Write-Output "Start adding Win-x64-GUI to .zip"
    Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../../Win-x64-GTiff2Tiles.GUI.zip *" -WorkingDirectory "Publish/GTiff2Tiles.GUI/win-x64/" | Out-Null
    Write-Output "Ended adding Win-x64-GUI to .zip"
}

# Add Win-x64-Console to .zip
Write-Output "Start adding Win-x64-Console to .zip"
Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../../Win-x64-GTiff2Tiles.Console.zip *" -WorkingDirectory "Publish/GTiff2Tiles.Console/win-x64/"
Write-Output "Ended adding Win-x64-Console to .zip"

# Add Linux-x64-Console to .zip
Write-Output "Start adding Linux-x64-Console to .zip"
Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../../Linux-x64-GTiff2Tiles.Console.zip *" -WorkingDirectory "Publish/GTiff2Tiles.Console/linux-x64/"
Write-Output "Ended adding Linux-x64-Console to .zip"

Write-Output "Ended adding published files to .zip archives"