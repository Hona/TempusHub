# Build the image with the name 'tempus-hub'
echo '-= Building Docker Image from Dockerfile =-\n'
docker build -t tempus-hub ./src/TempusHubBlazor/

echo '-= Runnning the Image (bound to host port 80) =-\n'
# Bind port '80:' of host to ':80' of container
docker run -d -p 80:80 -p 443:443 tempus-hub