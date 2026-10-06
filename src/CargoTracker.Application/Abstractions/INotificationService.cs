namespace CargoTracker.Application.Abstractions;

public interface INotificationService
{
    Task NotifyShipmentDelayedAsync(string trackingNumber, string receiverName,TimeSpan delay, CancellationToken cancellationToken = default);
}
