using CargoTracker.Application.Abstractions;
using CargoTracker.Domain.Entities;
using CargoTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CargoTracker.Infrastructure.Repositories;

public class EfShipmentRepository : IShipmentRepository
{
    private readonly CargoTrackerDbContext _dbContext;

    public EfShipmentRepository(CargoTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddAsync(Shipment shipment)
    {
        await _dbContext.Shipments.AddAsync(shipment);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Shipment>> GetAllAsync()
    {
        return await _dbContext.Shipments.ToListAsync();
    }

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
    {
        var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
        return shipment;
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
