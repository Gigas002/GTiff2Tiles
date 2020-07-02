# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build

WORKDIR /GTiff2Tiles
COPY . ./
# copy csproj and restore as distinct layers
RUN dotnet restore GTiff2Tiles.Console/GTiff2Tiles.Console.csproj

# copy and publish app and libraries
RUN dotnet publish GTiff2Tiles.Console/GTiff2Tiles.Console.csproj -c release -o /app /p:Platform=x64

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GTiff2Tiles.Console.dll"]