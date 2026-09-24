using CargoTracker.Domain.Enums;

namespace CargoTracker.Domain.Entities;

public class ShipmentStatusHistory
{
    public Guid Id { get; private set; }
    public Guid ShipmentId { get; private set; }
    public ShipmentStatus? FromStatus { get; private set; }
    public ShipmentStatus ToStatus { get; private set; }
    public DateTime ChangedAt { get; private set; }

    public ShipmentStatusHistory(Guid shipmentId, ShipmentStatus? fromStatus, ShipmentStatus toStatus)
    {
        Id = Guid.NewGuid();
        ShipmentId = shipmentId;
        FromStatus = fromStatus;
        ToStatus = toStatus;
        ChangedAt = DateTime.UtcNow;
    }
}
