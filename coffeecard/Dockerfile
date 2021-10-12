# runtime dependency
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# build dependency
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

# Copy the main source project files
COPY ./*.sln ./
COPY ./*/*.csproj ./
# Recreate project folder structure
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*}/ && mv $file ${file%.*}/; done

# Restore dependencies
RUN dotnet restore .

# Remainder of build process
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Move to runtime image
FROM base AS final
WORKDIR /app

# Copy compiled binaries
COPY --from=build /app/publish .

# Start WebAPI project
ENTRYPOINT ["dotnet", "CoffeeCard.WebApi.dll"]