namespace CargoTracker.Application.DTOs;

public record CreateShipmentRequest (string ReceiverName, string OriginCity, string DestinationCity, decimal WeightKg);




