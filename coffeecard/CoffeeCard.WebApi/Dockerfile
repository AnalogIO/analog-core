#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CoffeeCard.WebApi/CoffeeCard.WebApi.csproj", "CoffeeCard.WebApi/"]
COPY ["CoffeeCard.Common/CoffeeCard.Common.csproj", "CoffeeCard.Common/"]
COPY ["CoffeeCard.Library/CoffeeCard.Library.csproj", "CoffeeCard.Library/"]
COPY ["CoffeeCard.Models/CoffeeCard.Models.csproj", "CoffeeCard.Models/"]
COPY ["CoffeeCard.MobilePay/CoffeeCard.MobilePay.csproj", "CoffeeCard.MobilePay/"]
RUN dotnet restore "CoffeeCard.WebApi/CoffeeCard.WebApi.csproj"
COPY . .
WORKDIR "/src/CoffeeCard.WebApi"
RUN dotnet build "CoffeeCard.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoffeeCard.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeCard.WebApi.dll"]
