using CargoTracker.Application.Abstractions;
using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;

namespace CargoTracker.Api.Seeding;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        using var scope = services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (await users.GetByEmailAsync(email) is not null)
            return;

        var admin = new User(email, hasher.Hash(password), UserRole.Admin);
        await users.AddAsync(admin);
    }
}
