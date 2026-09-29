using CargoTracker.Application.DTOs;

namespace CargoTracker.Application.Abstractions;

public interface IShipmentService
{
    Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request, Guid? customerId);
    Task<ShipmentResponse?> GetByTrackingNumberAsync(string trackingNumber, Guid? requestingCustomerId);
    Task<IReadOnlyList<ShipmentResponse>> GetAllAsync();
    Task<IReadOnlyList<ShipmentResponse>> GetMyShipmentsAsync(Guid customerId);
    Task<ShipmentResponse?> UpdateStatusAsync(string trackingNumber, string newStatus);

}
