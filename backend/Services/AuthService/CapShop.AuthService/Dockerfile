FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Services/AuthService/CapShop.AuthService/CapShop.AuthService.csproj", "Services/AuthService/CapShop.AuthService/"]
RUN dotnet restore "Services/AuthService/CapShop.AuthService/CapShop.AuthService.csproj"
COPY . .
WORKDIR "/src/Services/AuthService/CapShop.AuthService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CapShop.AuthService.dll"]
