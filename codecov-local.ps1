$token=$args[0]

# WARNING! Using -c Release crashes coverlet results!

# For coverlet.msbuild
# dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Platform=x64 /p:Exclude="[GTiff2Tiles.*]GTiff2Tiles.Core.Localization.*" /p:Include="[GTiff2Tiles.Core]*"

# For coverlet.collector
dotnet test --collect:"XPlat Code Coverage" --settings runsettings.xml /p:Platform=x64

# codecov -f "GTiff2Tiles.Tests/coverage.opencover.xml" -t $token