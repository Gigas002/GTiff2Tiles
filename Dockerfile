# Select current dotnet version
FROM mcr.microsoft.com/dotnet/core/runtime:5.0
# FROM mcr.microsoft.com/dotnet/core/sdk:5.0

# copy from to
COPY GTiff2Tiles.Console/bin/x64/Release/netcoreapp5.0/publish GTiff2Tiles.Console/

ENTRYPOINT ["dotnet", "GTiff2Tiles.Console/GTiff2Tiles.Console.dll"]