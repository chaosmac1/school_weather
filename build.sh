# build backend
dotnet build ./BackendApi/BackendApi

# build frontend
cd ./frontend
npm run build
cd ../

# move build files to docker_files
rm -R ./docker_files/frontend
rm -R ./docker_files/backend
mkdir ./docker_files/frontend
mkdir ./docker_files/frontend/html
mkdir ./docker_files/backend
mkdir ./docker_files/backend/build
mv ./BackendApi/BackendApi/bin/Debug/net5.0/* ./docker_files/backend/build
mv ./frontend/build/* ./docker_files/frontend/html

cp ./docker_files/DockerfileBackend ./docker_files/backend/Dockerfile
cp ./docker_files/DockerfileFrontend ./docker_files/frontend/Dockerfile

cd ./docker_files
sudo docker-compose up -d --build
