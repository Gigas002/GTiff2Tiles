$buildAll=$args[0]

Write-Output "Started building/publishing"

# GTiff2Tiles.Core -- build and create nuget package

Write-Output "Start building GTiff2Tiles.Core"
dotnet publish "GTiff2Tiles.Core/GTiff2Tiles.Core.csproj" -c Release /p:Platform=x64
Write-Output "Ended building GTiff2Tiles.Core"


# win-x64
if ($IsWindows -or $buildAll)
{
    # GTiff2Tiles.Console
    Write-Output "Start win-x64-Console publish"
    dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r win-x64 /p:PublishDir=../Publish/GTiff2Tiles.Console/win-x64 /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true /p:PublishTrimmed=true
    Write-Output "Ended win-x64-Console publish"

    # GTiff2Tiles.GUI
    Write-Output "Start win-x64-GUI publish"
    dotnet publish "GTiff2Tiles.GUI/GTiff2Tiles.GUI.csproj" -c Release /p:PublishDir=../Publish/GTiff2Tiles.GUI/win-x64 /p:Platform=x64
    Write-Output "Ended win-x64-GUI publish"
}

# linux-x64
if ($IsLinux -or $buildAll)
{
    Write-Output "Start linux-x64-Console publish"
    dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r linux-x64 /p:PublishDir=../Publish/GTiff2Tiles.Console/linux-x64 /p:PublishSingleFile=true /p:IncludeAllContentForSelfExtract=true /p:PublishTrimmed=true
    Write-Output "Ended linux-x64-Console publish"
}

Write-Output "Ended building/publishing"

# Remove all *.pdb files
Write-Output "Removing all *.pdb files from Publish directory"
Get-ChildItem "Publish/" -Include *.pdb -Recurse | Remove-Item

# Copy docs, etc to published directories before zipping them
Write-Output "Copying docs, LICENSE.md, etc to published directories"

# win-x64
if ($IsWindows -or $buildAll)
{
    # GTiff2Tiles.Console
    Copy-Item -Path "GTiff2Tiles.Console/console-index.pdf" -Destination "Publish/GTiff2Tiles.Console/win-x64/console-index.pdf"
    Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.Console/win-x64/LICENSE.md"
    Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.Console/win-x64/CHANGELOG.md"

    # GTiff2Tiles.GUI
    Copy-Item -Path "GTiff2Tiles.GUI/gui-index.pdf" -Destination "Publish/GTiff2Tiles.GUI/win-x64/gui-index.pdf"
    Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.GUI/win-x64/LICENSE.md"
    Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.GUI/win-x64/CHANGELOG.md"
}

# linux-x64
if ($IsLinux -or $buildAll)
{
    # linux-x64-console
    Copy-Item -Path "GTiff2Tiles.Console/console-index.pdf" -Destination "Publish/GTiff2Tiles.Console/linux-x64/console-index.pdf"
    Copy-Item -Path "LICENSE.md" -Destination "Publish/GTiff2Tiles.Console/linux-x64/LICENSE.md"
    Copy-Item -Path "CHANGELOG.md" -Destination "Publish/GTiff2Tiles.Console/linux-x64/CHANGELOG.md"
}

# Add everything into archives. Requires installed 7z added to PATH.

Write-Output "Started adding published files to .zip archives"

# win-x64
if ($IsWindows -or $buildAll)
{
    # GTiff2Tiles.Console
    Write-Output "Start adding win-x64-Console to .zip"
    Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../../win-x64-GTiff2Tiles.Console.zip *" -WorkingDirectory "Publish/GTiff2Tiles.Console/win-x64/"
    Write-Output "Ended adding win-x64-Console to .zip"

    # GTiff2Tiles.GUI
    Write-Output "Start adding win-x64-GUI to .zip"
    Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../../win-x64-GTiff2Tiles.GUI.zip *" -WorkingDirectory "Publish/GTiff2Tiles.GUI/win-x64/"
    Write-Output "Ended adding win-x64-GUI to .zip"
}

# linux-x64
if ($IsLinux -or $buildAll)
{
    Write-Output "Start adding linux-x64-Console to .zip"
    Start-Process -NoNewWindow -Wait -FilePath "7z" -ArgumentList "a ../../linux-x64-GTiff2Tiles.Console.zip *" -WorkingDirectory "Publish/GTiff2Tiles.Console/linux-x64/"
    Write-Output "Ended adding linux-x64-Console to .zip"
}

Write-Output "Ended adding published files to .zip archives"