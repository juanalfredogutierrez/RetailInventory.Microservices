
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY AuthService/AuthService.csproj AuthService/
COPY BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/

RUN dotnet restore AuthService/AuthService.csproj

COPY . .

WORKDIR /src/AuthService

RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuthService.dll"]