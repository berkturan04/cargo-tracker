using CargoTracker.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace CargoTracker.Infrastructure.Notifications;

public class LoggingNotificationService : INotificationService
{
    private readonly ILogger<LoggingNotificationService> _logger;

    public LoggingNotificationService(ILogger<LoggingNotificationService> logger)
    {
        _logger = logger;
    }
    public Task NotifyShipmentDelayedAsync(string trackingNumber, string receiverName, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
           "[BİLDİRİM SİMÜLASYONU] {ReceiverName} alıcılı {TrackingNumber} takip numaralı kargo {DelayHours:F1} saattir beklemede. E-posta gönderildi (simüle edildi).",
           receiverName, trackingNumber, delay.TotalHours);
        return Task.CompletedTask;
    }
}
