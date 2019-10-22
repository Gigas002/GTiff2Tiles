# GTiff2Tiles.Core -- build and create nuget package

dotnet build "GTiff2Tiles.Core/GTiff2Tiles.Core.csproj" -c Release /p:Platform=x64


# GTiff2Tiles.Benhmarks


# Win-x64
dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Benchmarks/win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained

# Linux-x64
dotnet publish "GTiff2Tiles.Benchmarks/GTiff2Tiles.Benchmarks.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Benchmarks/linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained


# GTiff2Tiles.Console


# Win-x64
dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.Console/win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained

# Linux-x64
dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release -r linux-x64 -o Publish/GTiff2Tiles.Console/linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained


# GTiff2Tiles.GUI


# Win-x64
dotnet publish "GTiff2Tiles.GUI/GTiff2Tiles.GUI.csproj" -c Release -r win-x64 -o Publish/GTiff2Tiles.GUI/win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true --self-contained
