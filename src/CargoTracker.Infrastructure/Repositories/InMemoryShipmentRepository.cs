using System.Collections.Concurrent;
using CargoTracker.Application.Abstractions;
using CargoTracker.Domain.Entities;

namespace CargoTracker.Infrastructure.Repositories;

public class InMemoryShipmentRepository : IShipmentRepository
{
    private readonly ConcurrentDictionary<string, Shipment> _shipments = new();
    public Task AddAsync(Shipment shipment)
    {
        _shipments[shipment.TrackingNumber] = shipment;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Shipment>> GetAllAsync()
    {
        IReadOnlyList<Shipment> allShipments = _shipments.Values.ToList();
        return Task.FromResult(allShipments);
    }

    public Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
    {
        if(_shipments.TryGetValue(trackingNumber, out var shipment))
        {
            return Task.FromResult<Shipment?>(shipment);
        }
        return Task.FromResult<Shipment?>(null);
    }
}
