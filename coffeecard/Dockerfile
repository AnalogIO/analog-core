FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /sln

COPY ./*.sln ./

# Copy the main source project files
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done
# COPY * ./

RUN dotnet restore

# Remainder of build process
COPY . .
RUN dotnet build -c Release -o /app/build #--no-restore

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish #--no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoffeeCard.WebApi.dll"]
