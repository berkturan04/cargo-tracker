namespace CargoTracker.Application.DTOs;

public record CreateUserRequest(string Email, string Password, string Role);
