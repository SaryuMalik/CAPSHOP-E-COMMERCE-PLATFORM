FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Services/OrderService/CapShop.OrderService/CapShop.OrderService.csproj", "Services/OrderService/CapShop.OrderService/"]
COPY ["Shared/CapShop.Shared/CapShop.Shared.csproj", "Shared/CapShop.Shared/"]
RUN dotnet restore "Services/OrderService/CapShop.OrderService/CapShop.OrderService.csproj"
COPY . .
WORKDIR "/src/Services/OrderService/CapShop.OrderService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CapShop.OrderService.dll"]
