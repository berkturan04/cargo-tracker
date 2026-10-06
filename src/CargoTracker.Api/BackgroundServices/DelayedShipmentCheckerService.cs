using CargoTracker.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace CargoTracker.Api.BackgroundServices;

public class DelayedShipmentCheckerService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DelayedShipmentCheckerService> _logger;
    private readonly DelayedShipmentCheckerOptions _options;

    public DelayedShipmentCheckerService(
        IServiceProvider services,
        ILogger<DelayedShipmentCheckerService> logger,
        IOptions<DelayedShipmentCheckerOptions> options)
    {
        _services = services;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Gecikmiş kargo kontrol servisi başladı. Her {Interval} dakikada bir, {Threshold} saatten eski kargoları kontrol edecek.",
            _options.IntervalMinutes, _options.DelayThresholdHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckDelayedShipmentsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gecikmiş kargo kontrolü sırasında hata oluştu.");
            }

            await Task.Delay(TimeSpan.FromMinutes(_options.IntervalMinutes), stoppingToken);
        }
    }

    private async Task CheckDelayedShipmentsAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var shipmentRepository = scope.ServiceProvider.GetRequiredService<IShipmentRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        var threshold = TimeSpan.FromHours(_options.DelayThresholdHours);
        var delayedShipments = await shipmentRepository.GetDelayedUnnotifiedAsync(threshold, ct);

        foreach (var shipment in delayedShipments)
        {
            var delay = DateTime.UtcNow - shipment.CreatedAt;
            await notificationService.NotifyShipmentDelayedAsync(shipment.TrackingNumber, shipment.ReceiverName, delay, ct);
            shipment.MarkDelayNotified();
        }

        if (delayedShipments.Count > 0)
        {
            await shipmentRepository.SaveChangesAsync();
            _logger.LogInformation("{Count} gecikmiş kargo için bildirim gönderildi.", delayedShipments.Count);
        }
    }
}
