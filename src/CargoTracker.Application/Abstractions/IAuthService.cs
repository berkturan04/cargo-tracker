using CargoTracker.Application.DTOs;

namespace CargoTracker.Application.Abstractions;

public interface IAuthService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request);
}
