FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["NotificationService/CapShop.NotificationService/CapShop.NotificationService/CapShop.NotificationService.csproj", "NotificationService/CapShop.NotificationService/CapShop.NotificationService/"]
COPY ["Shared/CapShop.Shared/CapShop.Shared.csproj", "Shared/CapShop.Shared/"]
RUN dotnet restore "NotificationService/CapShop.NotificationService/CapShop.NotificationService/CapShop.NotificationService.csproj"
COPY . .
WORKDIR "/src/NotificationService/CapShop.NotificationService/CapShop.NotificationService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CapShop.NotificationService.dll"]
