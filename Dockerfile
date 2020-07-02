# Select current dotnet version
# FROM mcr.microsoft.com/dotnet/core/runtime:5.0
# FROM mcr.microsoft.com/dotnet/core/sdk:5.0

# dotnet publish console app
# RUN dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release /p:Platform=x64

# copy from to
# COPY GTiff2Tiles.Console/bin/x64/Release/netcoreapp5.0/publish GTiff2Tiles.Console/

# ENTRYPOINT ["dotnet", "GTiff2Tiles.Console/GTiff2Tiles.Console.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:5.0 AS build-env
RUN ls -a
WORKDIR /.
RUN ls -a

# Copy csproj and restore as distinct layers
COPY GTiff2Tiles.Console/GTiff2Tiles.Console.csproj ./
RUN dotnet restore
RUN ls -a

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out /p:Platform=x64
RUN ls -a

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:5.0
WORKDIR /.
RUN ls -a
COPY --from=build-env /./out .
RUN ls -a
ENTRYPOINT ["dotnet", "GTiff2Tiles.Console/GTiff2Tiles.Console.dll"]