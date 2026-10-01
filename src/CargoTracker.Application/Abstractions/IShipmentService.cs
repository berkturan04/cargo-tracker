using CargoTracker.Application.DTOs;

namespace CargoTracker.Application.Abstractions;

public interface IShipmentService
{
    Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request, Guid? customerId);
    Task<ShipmentResponse?> GetByTrackingNumberAsync(string trackingNumber, Guid? requestingCustomerId);
    Task<PagedResult<ShipmentResponse>> GetAllAsync(ShipmentQuery query);
    Task<PagedResult<ShipmentResponse>> GetMyShipmentsAsync(Guid customerId, ShipmentQuery query);
    Task<PagedResult<ShipmentResponse>> GetAssignedToMeAsync(Guid courierId, ShipmentQuery query);
    Task<ShipmentResponse?> UpdateStatusAsync(string trackingNumber, string newStatus, Guid? requestingCourierId);
    Task<ShipmentResponse?> AssignCourierAsync(string trackingNumber, Guid courierId);

}
