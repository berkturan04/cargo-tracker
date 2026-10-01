using CargoTracker.Domain.Entities;

namespace CargoTracker.Application.Abstractions;

public interface IShipmentRepository
{
    Task AddAsync(Shipment shipment);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
    Task SaveChangesAsync();
    Task<(IReadOnlyList<Shipment> Items, int TotalCount)> GetFilteredAsync(ShipmentFilter filter);

}
