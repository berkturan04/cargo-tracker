# ---- Derleme (build) aşaması ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Önce sadece .csproj dosyalarını kopyala: paket referansları değişmediği sürece
# "dotnet restore" katmanı Docker'ın önbelleğinden gelsin, her kod değişikliğinde yeniden
# indirme yapılmasın.
COPY src/CargoTracker.Domain/*.csproj src/CargoTracker.Domain/
COPY src/CargoTracker.Application/*.csproj src/CargoTracker.Application/
COPY src/CargoTracker.Infrastructure/*.csproj src/CargoTracker.Infrastructure/
COPY src/CargoTracker.Api/*.csproj src/CargoTracker.Api/
RUN dotnet restore src/CargoTracker.Api/CargoTracker.Api.csproj

# Şimdi kodun tamamını kopyala ve yayınla (publish)
COPY src/ src/
RUN dotnet publish src/CargoTracker.Api/CargoTracker.Api.csproj -c Release -o /app/publish --no-restore

# ---- Çalışma zamanı (runtime) aşaması ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "CargoTracker.Api.dll"]