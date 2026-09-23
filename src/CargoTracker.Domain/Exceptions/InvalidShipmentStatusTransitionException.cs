using CargoTracker.Domain.Enums;

namespace CargoTracker.Domain.Exceptions;

public class InvalidShipmentStatusTransitionException : Exception
{
    public InvalidShipmentStatusTransitionException(ShipmentStatus fromStatus, ShipmentStatus toStatus)
        : base($"Invalid shipment status transition from {fromStatus} to {toStatus}.")
    {
    }
}
