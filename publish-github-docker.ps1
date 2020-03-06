$GH_TOKEN=$args[0]
$IMAGE_ID=$args[1]
if($GH_TOKEN) {    
    if ($IMAGE_ID) {
    docker login -u Gigas002 -p $GH_TOKEN docker.pkg.github.com
    docker tag $IMAGE_ID docker.pkg.github.com/gigas002/gtiff2tiles/gtiff2tiles-console:latest
    docker push docker.pkg.github.com/gigas002/gtiff2tiles/gtiff2tiles-console:latest
    }
    else {
        Write-Host "Please, specify IMAGE_ID"           
    }
} else {            
    Write-Host "Please, specify GH_TOKEN"           
}

docker login -u Gigas002 -p $GH_TOKEN docker.pkg.github.com
docker tag $IMAGE_ID docker.pkg.github.com/gigas002/gtiff2tiles/gtiff2tiles-console:latest
docker push docker.pkg.github.com/gigas002/gtiff2tiles/gtiff2tiles-console:lates