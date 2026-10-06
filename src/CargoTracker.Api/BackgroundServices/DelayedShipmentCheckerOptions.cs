namespace CargoTracker.Api.BackgroundServices;

public class DelayedShipmentCheckerOptions
{
    public int IntervalMinutes { get; set; } = 5;
    public int DelayThresholdHours { get; set; } = 48;
}
