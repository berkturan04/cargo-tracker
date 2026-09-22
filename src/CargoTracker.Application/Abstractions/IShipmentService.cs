using CargoTracker.Application.DTOs;

namespace CargoTracker.Application.Abstractions;

public interface IShipmentService
{
    Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request);
    Task<ShipmentResponse?> GetByTrackingNumberAsync(string trackingNumber);
    Task<IReadOnlyList<ShipmentResponse>> GetAllAsync();

}
