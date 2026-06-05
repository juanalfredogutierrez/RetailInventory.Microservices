FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ProductoService/ProductoService.csproj ProductoService/
COPY BuildingBlocks/BuildingBlocks.csproj BuildingBlocks/

RUN dotnet restore ProductoService/ProductoService.csproj

COPY . .

WORKDIR /src/ProductoService

RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProductoService.dll"]