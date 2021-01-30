# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /GTiff2Tiles

# copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore GTiff2Tiles.Console/GTiff2Tiles.Console.csproj -r linux-x64

# copy and publish app and libraries
RUN dotnet publish GTiff2Tiles.Console/GTiff2Tiles.Console.csproj -c Release -o /app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GTiff2Tiles.Console.dll"]