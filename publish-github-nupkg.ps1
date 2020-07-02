# packageName is a FileInfo.Name, not FullName
$packageName=$args[0]
if($packageName)
{       
    dotnet build "GTiff2Tiles.Core/GTiff2Tiles.Core.csproj" -c Release /p:Platform=x64
    dotnet nuget push "GTiff2Tiles.Core/GTiff2Tiles.Core/bin/x64/Release/$packageName" -s "github" -ss "github"
}
else
{            
    Write-Host "Please, specify .nupkg file name (not full path!)"           
}