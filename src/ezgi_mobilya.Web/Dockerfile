FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ezgi_mobilya.Web/ezgi_mobilya.Web.csproj", "src/ezgi_mobilya.Web/"]
COPY ["src/ezgi_mobilya.Service/ezgi_mobilya.Service.csproj", "src/ezgi_mobilya.Service/"]
COPY ["src/ezgi_mobilya.Data/ezgi_mobilya.Data.csproj", "src/ezgi_mobilya.Data/"]
COPY ["src/ezgi_mobilya.Core/ezgi_mobilya.Core.csproj", "src/ezgi_mobilya.Core/"]
RUN dotnet restore "./src/ezgi_mobilya.Web/ezgi_mobilya.Web.csproj"
COPY . .
WORKDIR "/src/src/ezgi_mobilya.Web"
RUN dotnet build "./ezgi_mobilya.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ezgi_mobilya.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ezgi_mobilya.Web.dll"]
