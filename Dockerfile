FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY global.json Directory.Build.props .editorconfig ./
COPY src/DoorstepCO.Core/DoorstepCO.Core.csproj src/DoorstepCO.Core/
COPY src/DoorstepCO.Api/DoorstepCO.Api.csproj src/DoorstepCO.Api/
RUN dotnet restore src/DoorstepCO.Api/DoorstepCO.Api.csproj

COPY src/ src/
RUN dotnet publish src/DoorstepCO.Api/DoorstepCO.Api.csproj --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "DoorstepCO.Api.dll"]
