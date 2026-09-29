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
        return await CreateUserInternalAsync(request.Email, request.Password, UserRole.Customer);
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

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role) || !Enum.IsDefined(role))
            throw new ArgumentException($"Geçersiz rol: {request.Role}");

        return await CreateUserInternalAsync(request.Email, request.Password, role);
    }
    private async Task<UserResponse> CreateUserInternalAsync(string email, string password, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            throw new ArgumentException("Şifre en az 8 karakter olmalıdır.", nameof(password));

        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
            throw new EmailAlreadyInUseException(email);

        var user = new User(email, _passwordHasher.Hash(password), role);
        await _userRepository.AddAsync(user);

        return MapToResponse(user);
    }
}
