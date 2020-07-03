# Package version, e.g. 2.0.0-ci303
$version=$args[0]
if($packageName)
{       
    dotnet build "GTiff2Tiles.Core/GTiff2Tiles.Core.csproj" -c Release /p:Platform=x64
    dotnet nuget push "GTiff2Tiles.Core/GTiff2Tiles.Core/bin/x64/Release/GTiff2Tiles.$version.nupkg" -s "github" -ss "github"
}
else
{            
    Write-Host "Please, specify package version!"           
}