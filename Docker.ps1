# Docker create image

# dotnet publish console app
# /p:PublishReadyToRun=true /p:PublishTrimmed=true /p:PublishSingleFile=true
dotnet publish "GTiff2Tiles.Console/GTiff2Tiles.Console.csproj" -c Release /p:Platform=x64

# docker build
docker build -t gtiff2tiles-console -f Dockerfile .

# check image
# docker images
# docker run -it --rm gtiff2tiles-console