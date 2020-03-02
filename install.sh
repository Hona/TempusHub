# Stop and remove existing production container
echo '-= Stopping TempusHub Production Container =-'
docker container stop tempus-hub-production

echo '-= Removing Old TempusHub Production Container =-'
docker container rm tempus-hub-production

# Build the image with the name 'tempus-hub'
echo '-= Building Docker Image from Dockerfile =-\n'
docker build -t tempus-hub ./src/TempusHubBlazor/

echo '-= Runnning the Image (bound to host port 80) =-\n'
# Bind port '80:' of host to ':80' of container
docker run --name "tempus-hub-production" -d -p 80:80 -p 443:443 --env-file env-vars.env tempus-hub