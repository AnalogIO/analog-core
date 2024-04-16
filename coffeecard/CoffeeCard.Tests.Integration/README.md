## Auto generated strongly typed http client for Core Api
This project only contains the configuration required to generate strongly typed http clients that we use to test the Api. 

This is set up by configuring the OpenApiReference to point to the outputted openApi specs of the Core Api. The core api is configured as a build dependency of this project on the solution level.

The clients that are out putted are:
- CoffeeCardClient
- CoffeeCardClientV2

See the following for more info in how to set up, and configure clients
- [Microsoft reference](https://learn.microsoft.com/en-us/aspnet/core/web-api/microsoft.dotnet-openapi?view=aspnetcore-8.0)
- [Blog explaining in detail](https://stevetalkscode.co.uk/openapireference-commands) ([Backup archive.org](https://web.archive.org/web/20240415183726/https://stevetalkscode.co.uk/openapireference-commands))
