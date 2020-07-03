$GH_TOKEN=$args[0]
if($GH_TOKEN)
{    
    docker login -u Gigas002 -p $GH_TOKEN docker.pkg.github.com
    docker build -t docker.pkg.github.com/gigas002/gtiff2tiles/gtiff2tiles-console:latest -f Dockerfile .
    docker push docker.pkg.github.com/gigas002/gtiff2tiles/gtiff2tiles-console:latest
}
else
{            
    Write-Host "Please, specify GH_TOKEN"           
}