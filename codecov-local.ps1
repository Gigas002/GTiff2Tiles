$token=$args[0]

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Platform=x64 -c Release
# codecov -f "GTiff2Tiles.Tests/coverage.opencover.xml" -t $token