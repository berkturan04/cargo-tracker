using CargoTracker.Domain.Entities;

namespace CargoTracker.Application.Abstractions;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
}
