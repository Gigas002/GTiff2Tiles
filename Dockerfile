# Select current dotnet version
FROM mcr.microsoft.com/dotnet/core/runtime:3.1

# copy from to
COPY GTiff2Tiles.Console/bin/x64/Release/netcoreapp3.1/publish GTiff2Tiles.Console/

ENTRYPOINT ["dotnet", "GTiff2Tiles.Console/GTiff2Tiles.Console.dll"]