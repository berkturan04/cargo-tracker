using CargoTracker.Application.Abstractions;
using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;
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

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
    {
        var shipment = await _dbContext.Shipments.FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
        return shipment;
    }

    public async Task<IReadOnlyList<Shipment>> GetDelayedUnnotifiedAsync(TimeSpan threshold, CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow - threshold;

        return await _dbContext.Shipments
            .Where(s => s.DelayNotifiedAt == null &&
                        s.Status != ShipmentStatus.Delivered &&
                        s.Status != ShipmentStatus.Returned &&
                        s.CreatedAt < cutoff)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Shipment> Items, int TotalCount)> GetFilteredAsync(ShipmentFilter filter)
    {
        var query = _dbContext.Shipments.AsNoTracking().AsQueryable();

        if (filter.CustomerId.HasValue)
            query = query.Where(s => s.CustomerId == filter.CustomerId.Value);

        if (filter.CourierId.HasValue)
            query = query.Where(s => s.CourierId == filter.CourierId.Value);

        if (filter.Status.HasValue)
            query = query.Where(s => s.Status == filter.Status.Value);

        if (!string.IsNullOrWhiteSpace(filter.City))
            query = query.Where(s => s.OriginCity.Contains(filter.City) || s.DestinationCity.Contains(filter.City));

        if (filter.FromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(s => s.CreatedAt <= filter.ToDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}
