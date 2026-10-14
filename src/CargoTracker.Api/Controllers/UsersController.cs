using CargoTracker.Application;
using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTracker.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }


    /// <summary>Admin, Customer veya Courier rolünde yeni bir kullanıcı oluşturur.</summary>
    /// <remarks>Sadece Admin rolü erişebilir.</remarks>
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request)
    {
        var response = await _authService.CreateUserAsync(request);
        return Ok(response);
    }
}
