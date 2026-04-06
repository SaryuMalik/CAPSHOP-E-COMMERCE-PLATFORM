FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Services/CatalogService/CapShop.CatalogService/CapShop.CatalogService.csproj", "Services/CatalogService/CapShop.CatalogService/"]
RUN dotnet restore "Services/CatalogService/CapShop.CatalogService/CapShop.CatalogService.csproj"
COPY . .
WORKDIR "/src/Services/CatalogService/CapShop.CatalogService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CapShop.CatalogService.dll"]
