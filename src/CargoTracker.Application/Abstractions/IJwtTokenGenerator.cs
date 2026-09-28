using CargoTracker.Application.DTOs;
using CargoTracker.Domain.Entities;

namespace CargoTracker.Application.Abstractions;

public interface IJwtTokenGenerator
{
    AccessToken Generate(User user);
}
