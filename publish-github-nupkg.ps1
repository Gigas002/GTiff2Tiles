$packageName=$args[0]
if($packageName) {            
    dotnet pack "GTiff2Tiles.Core/GTiff2Tiles.Core.csproj" --configuration Release /p:Platform=x64
    dotnet nuget push "GTiff2Tiles.Core/GTiff2Tiles.Core/bin/x64/Release/$packageName" --source "github"
} else {            
    Write-Host "Please, specify .nupkg file name (not full path!)"           
}
