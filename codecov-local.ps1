$token=$args[0]

dotnet test "GTiff2Tiles.Tests/GTiff2Tiles.Tests.csproj" /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Platform=x64 -c Release
# codecov -f coverage.opencover.xml -t $token