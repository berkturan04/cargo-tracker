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
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
        
    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
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

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, request.Password))
            throw new InvalidCredentialsException();

        var token = _jwtTokenGenerator.Generate(user);
        return new LoginResponse(token.Token, token.ExpiresAt, user.Email, user.Role.ToString());
    }
}
