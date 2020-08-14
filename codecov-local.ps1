$token=$args[0]

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:Platform=x64 -c Release
# codecov -f coverage.opencover.xml -t $token