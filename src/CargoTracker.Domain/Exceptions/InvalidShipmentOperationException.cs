namespace CargoTracker.Domain.Exceptions;

public class InvalidShipmentOperationException : Exception
{
    public InvalidShipmentOperationException(string message) : base(message)
    {
    }
}
