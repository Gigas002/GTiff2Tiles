$token=$args[0]

# WARNING! Using -c Release crashes coverlet results!

# For coverlet.msbuild, only win
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Platform=x64 /p:Exclude="[GTiff2Tiles.*]GTiff2Tiles.Core.Localization.*" /p:Include="[GTiff2Tiles.Core]*"

# For coverlet.collector, both win and linux. Check out the runsettings.xml if you'll run this
# dotnet test --collect:"XPlat Code Coverage" --settings runsettings.xml /p:Platform=x64

# Upload coverage analyze results to codecov. Requires dotnet codecov tool (installable through nuget)
codecov -f "GTiff2Tiles.Tests/coverage.opencover.xml" -t $token