FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
EXPOSE 8080
# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Api.dll"]






#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081

# Copy everything
#COPY . ./
# Restore as distinct layers
#RUN dotnet restore
# Build and publish a release
#RUN dotnet publish -c Release -o out

# Build runtime image
#FROM mcr.microsoft.com/dotnet/aspnet:8.0
#WORKDIR /app
#COPY --from=build-env /app/out .
#ENTRYPOINT ["dotnet", "MinimalApi.dll"]

#docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin sonederbr/keycloak-study-api:0.1 start-dev
#docker run --rm -it -p 5000:8080 aspnetapp

#docker build -t mapi -f Dockerfile .
#docker create --name miniapi mapi
#docker run --rm -ti -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_HTTP_PORTS=8080 -p 8080:8080 mapi  # http://localhost:8080/
#docker run --rm -ti -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_HTTP_PORTS=8080 -p 8081:8080 mapi  # http://localhost:8081/

#dotnet .\MinimalApi.dll --environment=Development --urls=http://localhost:8081/

#https://github.com/dotnet/dotnet-docker/tree/main/samples/aspnetapp
#https://www.makeuseof.com/docker-image-dot-net-web-api/
#https://stackoverflow.com/questions/48298284/merging-appsettings-with-environment-variables-in-net-core
