FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MobileMonitoringBackend/MobileMonitoringBackend.csproj", "MobileMonitoringBackend/"]
RUN dotnet restore "MobileMonitoringBackend/MobileMonitoringBackend.csproj"
COPY . .
WORKDIR "/src/MobileMonitoringBackend"
RUN dotnet build "MobileMonitoringBackend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MobileMonitoringBackend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MobileMonitoringBackend.dll"]
