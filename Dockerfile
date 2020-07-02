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
COPY GTiff2Tiles.Core/GTiff2Tiles.Core.csproj .
RUN ls -a
RUN dotnet restore GTiff2Tiles.Console.csproj
RUN ls -a

# copy and publish app and libraries
COPY . ./
RUN dotnet publish -c release -o /app --no-restore /p:Platform=x64
RUN ls -a

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:5.0
WORKDIR /.
RUN ls -a
COPY --from=build /app .
ENTRYPOINT ["dotnet", "GTiff2Tiles.Console.dll"]