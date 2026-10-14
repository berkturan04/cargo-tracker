using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(IAuthService authService, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    /// <summary>Yeni bir müşteri hesabı oluşturur.</summary>
    /// <remarks>Açık kayıt her zaman Customer rolü atar; Admin/Courier hesapları sadece bir Admin tarafından <c>POST /api/users</c> ile oluşturulabilir.</remarks>
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));

        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    /// <summary>E-posta ve şifreyle giriş yapar, JWT token döner.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));

        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }
    
    /// <summary>Geçerli token'daki claim'leri (sub, email, role vb.) gösterir.</summary>
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }
}
