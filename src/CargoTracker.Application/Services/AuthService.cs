using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using CargoTracker.Application.Exceptions;
using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;

namespace CargoTracker.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
        
    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
    }
    public async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser is not null)
            throw new EmailAlreadyInUseException(request.Email);
        
        var role = Enum.Parse<UserRole>(request.Role, ignoreCase: true);

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var user = new User(
        email: request.Email,
        passwordHash: hashedPassword,
        role: role
    );
        await _userRepository.AddAsync(user);

        return MapToResponse(user);
        
    }
    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse(user.Id, user.Email, user.Role.ToString(), user.CreatedAt);
    }
}
