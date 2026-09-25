using CargoTracker.Application.Abstractions;
using CargoTracker.Domain.Entities;
using CargoTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CargoTracker.Infrastructure.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly CargoTrackerDbContext _dbContext;

    public EfUserRepository(CargoTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLowerInvariant());
        return user;
    }
}
