## Proje Hakkında
Kargo/sevkiyat takip sistemi. Bir e-ticaret şirketinin siparişlerinin kargoya
verilmesinden teslimata kadar durumunu takip eden bir REST API.

## Durum
🚧 Geliştirme aşamasında (Hafta 4/8 tamamlandı)

## Tamamlanan özellikler
- Katmanlı mimari (Domain, Application, Infrastructure, Api)
- PostgreSQL + EF Core ile kalıcı veri
- Durum makinesi (kargo takip geçmişiyle birlikte)
- JWT authentication, rol bazlı yetkilendirme (Admin, Courier, Customer)
- Müşteri sahiplik izolasyonu
- xUnit ile domain testleri

## Teknolojiler
- .NET 10 Web API
- Katmanlı mimari (Domain, Application, Infrastructure, Api)

## Çalıştırma
dotnet run --project src/CargoTracker.Api
