FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Ecoeden.Notification.Service/Ecoeden.Notification.Service.csproj", "Ecoeden.Notification.Service/"]
RUN dotnet restore "Ecoeden.Notification.Service/Ecoeden.Notification.Service.csproj"
COPY . .
WORKDIR "/src/Ecoeden.Notification.Service"
RUN dotnet build "Ecoeden.Notification.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ecoeden.Notification.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecoeden.Notification.Service.dll"]