namespace CargoTracker.Application.DTOs;

public record UserResponse(Guid Id, string Email, string Role, DateTime CreatedAt);
