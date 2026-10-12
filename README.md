# cargo-tracker

[![CI](https://github.com/berkturan04/cargo-tracker/actions/workflows/ci.yml/badge.svg)](https://github.com/berkturan04/cargo-tracker/actions/workflows/ci.yml)

## Durum
✅ Hafta 7/8 tamamlandı — arka plan servisi, cache, entegrasyon testleri ve CI hazır

## Teknolojiler
- .NET 10 Web API, katmanlı mimari (Domain, Application, Infrastructure, Api)
- PostgreSQL + EF Core (code-first migration'lar)
- Redis (cache-aside deseni)
- JWT authentication, rol bazlı yetkilendirme (Admin, Courier, Customer)
- FluentValidation ile girdi doğrulama
- Merkezi hata yönetimi (IExceptionHandler + ProblemDetails/RFC 9457)
- Serilog ile yapılandırılmış loglama (konsol + dosya sink'leri)
- BackgroundService ile gecikmiş kargo tespiti ve bildirim simülasyonu
- xUnit ile domain testleri + WebApplicationFactory/Testcontainers ile entegrasyon testleri
- Docker (çok aşamalı build) + Docker Compose (api + postgres + redis)
- GitHub Actions ile CI (build, test, Dockerfile doğrulama)

## Çalıştırma

**Tek komutla (önerilen):**
```bash
cp .env.example .env   # değerleri doldur
docker compose up --build
```
API `http://localhost:8080/scalar` adresinde açılır.

**Geliştirme modunda:**
```bash
docker compose up -d postgres redis
dotnet run --project src/CargoTracker.Api
```

## Tamamlanan özellikler
- Kargo oluşturma, takip, durum makinesi (Created → AtBranch → InTransit → OutForDelivery → Delivered/Returned)
- Her durum değişikliğinin geçmişte tutulması
- Kayıt/giriş, JWT ile kimlik doğrulama
- Üç rol: Admin (tüm yetkiler), Courier (atandığı kargoları yönetir), Customer (kendi kargolarını görür)
- Kurye atama, müşteri/kurye sahiplik izolasyonu
- Durum/şehir/tarih filtreleme ve sayfalama
- FluentValidation ile girdi doğrulama (RFC 9457 ProblemDetails formatında hatalar)
- Yapılandırılmış loglama (Serilog)
- Gecikmiş kargoları tespit eden arka plan servisi + bildirim simülasyonu
- Redis ile cache-aside deseni (takip sorgusu, durum/kurye değişikliğinde geçersiz kılınıyor)
- 15 domain testi + 10 entegrasyon testi (gerçek Postgres/Redis container'larına karşı, toplam 25)
- Docker ile tam sistem (`docker compose up`), GitHub Actions ile CI

## Yapılacaklar (sırada)
- README'nin son hâli: mimari diyagramı, tasarım kararları
- Canlı demo