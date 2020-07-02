# Select current dotnet version
# FROM mcr.microsoft.com/dotnet/core/runtime:5.0
# FROM mcr.microsoft.com/dotnet/core/sdk:5.0

# dotnet publish console app
# RUN dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release /p:Platform=x64

# copy from to
# COPY GTiff2Tiles.Console/bin/x64/Release/netcoreapp5.0/publish GTiff2Tiles.Console/

# ENTRYPOINT ["dotnet", "GTiff2Tiles.Console/GTiff2Tiles.Console.dll"]

# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY GTiff2Tiles.Console/GTiff2Tiles.Console.csproj .
RUN dotnet restore

# copy and publish app and libraries
COPY . ./
RUN dotnet publish -c Release -o out /p:Platform=x64

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:5.0
WORKDIR /.
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GTiff2Tiles.Console.dll"]