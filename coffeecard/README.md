# Coffeecard ASP.NET Core solution

## Project structure

| Project                                 	| Description                                                                                                      	|
|-----------------------------------------	|------------------------------------------------------------------------------------------------------------------	|
| `CoffeeCard.Common`                     	| Commons project for classes which all projects have in common e.g. settings files, constants, common exceptions. 	|
| `CoffeeCard.Library`                    	| Library project for the main business logic, e.g. Services, Persistence.                                         	|
| `CoffeeCard.MobilePay`                  	| Domain specific project for MobilePay and communication with their backend.                                      	|
| `CoffeeCard.MobilePay.RefundConsoleApp` 	| ConsoleApp for refunding MobilePay transactions.                                                                 	|
| `CoffeeCard.Models`                     	| Models and DTO's classes                                                                                         	|
| `CoffeeCard.Tests.Integration`          	| Integration tests                                                                                                	|
| `CoffeeCard.Tests.Unit`                 	| Unit tests                                                                                                       	|
| `CoffeeCard.WebApi`                     	| WebAPI project with controllers and API specific classes.  

## Configuration

The CoffeeCard WebApi solution has a few runtime dependencies. The following prerequisites are expected to be present in the root of the `CoffeeCard.WebApi` project folder at runtime:

- Application configuration (`appsettings.json`)
- MobilePay certificate for backend communications

Local development versions of the above are checked into the codebase.

## Run the solution locally

The Coffeecard WebApi solution can be built and run locally using Docker. A `docker-compose` file is provided in the coffeecard folder. The Docker Compose contains a MSSQL database server image and a reference to the WebApi Dockerfile.

The solution will use the MSSQL database run in the Docker Compose if the default `appsettings.json` configuration file is used. The WebApi will be available at `localhost:8080`.

The solution is built and run using the following commands
```bash
docker-compose build
docker-compose up
```

### Setting up https

To run the container over HTTPS, a self-signed certificate has to be created. This can setup using the guide [Developing ASP.NET Core Applications with Docker over HTTPS](https://github.com/dotnet/dotnet-docker/blob/main/samples/run-aspnetcore-https-development.md#building-and-running-the-sample-with-https) or using the commands:
```bash
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\CoffeeCard.WebApi.pfx -p crypticpassword
dev-certs https --trust # might be different on Linux distros
user-secrets -p .\CoffeeCard.WebApi\CoffeeCard.WebApi.csproj set "Kestrel:Certificates:Development:Password" "crypticpassword"
```

Run the image
```bash
docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_ENVIRONMENT=Development -v $env:APPDATA\microsoft\UserSecrets\:/root/.microsoft/usersecrets -v $env:USERPROFILE\.aspnet\https:/root/.aspnet/https/ coffeecard-webapi
```