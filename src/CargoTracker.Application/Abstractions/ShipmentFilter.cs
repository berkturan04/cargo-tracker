using CargoTracker.Domain.Enums;

namespace CargoTracker.Application.Abstractions;

public class ShipmentFilter
{
    public Guid? CustomerId { get; set; }
    public Guid? CourierId { get; set; }
    public ShipmentStatus? Status { get; set; }
    public string? City { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
