using CargoTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Xunit;

namespace CargoTracker.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("cargotracker_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:8")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:CargoTrackerDb", _postgresContainer.GetConnectionString());
        builder.UseSetting("ConnectionStrings:Redis", _redisContainer.GetConnectionString());
        builder.UseSetting("Jwt:Key", "test-ortami-icin-sadece-imzalama-anahtari-min-32-karakter");
        builder.UseSetting("SeedAdmin:Email", "admin@test.local");
        builder.UseSetting("SeedAdmin:Password", "TestAdmin123!");
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        await _redisContainer.StartAsync();

        // Services'e (ve dolayısıyla Program.cs'in tamamına, AdminSeeder dahil) hiç
        // dokunmadan, migration'ları container'a BAĞIMSIZ bir bağlantıyla uyguluyoruz.
        // Services'e ilk erişim host'u gerçekten çalıştırır; o an AdminSeeder de tetiklenir
        // ve migration'lar henüz uygulanmamışsa "Users tablosu yok" hatası alırız.
        var optionsBuilder = new DbContextOptionsBuilder<CargoTrackerDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString());

        await using var migrationContext = new CargoTrackerDbContext(optionsBuilder.Options);
        await migrationContext.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}