# analog-core

![Build and test solution](https://github.com/AnalogIO/analog-core/workflows/Build%20and%20test%20solution/badge.svg)

This repository will contain the ASP.NET Core 2 version of the backend for the coffeecard apps.

Lets try to follow this guide: https://docs.microsoft.com/en-us/aspnet/core/tutorials/web-api-vsc?view=aspnetcore-2.1

Add `appsettings.json` to directory `/coffeecard/` with the following format:
```json
{
  "ConnectionStrings": {
    "CoffeecardDatabase": "<connectionstring>"  
  },
  "TokenKey": "<symmetrickey>"
}
```