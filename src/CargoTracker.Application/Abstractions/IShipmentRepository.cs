using CargoTracker.Domain.Entities;

namespace CargoTracker.Application.Abstractions;

public interface IShipmentRepository
{
    Task AddAsync(Shipment shipment);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
    Task<IReadOnlyList<Shipment>> GetAllAsync();

}
