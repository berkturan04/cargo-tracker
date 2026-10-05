## Durum
🚧 Geliştirme aşamasında (Hafta 5/8 tamamlandı)

## Teknolojiler
- .NET 10 Web API, katmanlı mimari (Domain, Application, Infrastructure, Api)
- PostgreSQL + EF Core (code-first migration'lar)
- JWT authentication, rol bazlı yetkilendirme (Admin, Courier, Customer)
- FluentValidation ile girdi doğrulama
- Merkezi hata yönetimi (IExceptionHandler + ProblemDetails/RFC 9457)
- Serilog ile yapılandırılmış loglama (konsol + dosya sink'leri)
- xUnit ile domain testleri
- Docker Compose (PostgreSQL)

## Tamamlanan özellikler
- Kargo oluşturma, takip, durum makinesi (Created → AtBranch → InTransit → OutForDelivery → Delivered/Returned)
- Her durum değişikliğinin geçmişte tutulması
- Kayıt/giriş, JWT ile kimlik doğrulama
- Üç rol: Admin (tüm yetkiler), Courier (atandığı kargoları yönetir), Customer (kendi kargolarını görür)
- Kurye atama, müşteri/kurye sahiplik izolasyonu
- Durum/şehir/tarih filtreleme ve sayfalama
- FluentValidation ile girdi doğrulama (RFC 9457 ProblemDetails formatında hatalar)
- Yapılandırılmış loglama (Serilog)

## Yapılacaklar (sırada)
- Arka plan servisi (gecikmiş kargo tespiti), Redis cache
- Entegrasyon testleri, CI (GitHub Actions)
- README'nin tam hâli, canlı demo