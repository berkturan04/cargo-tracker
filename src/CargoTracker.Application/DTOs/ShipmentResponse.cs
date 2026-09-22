namespace CargoTracker.Application.DTOs;

public record ShipmentResponse(Guid Id, string TrackingNumber, string ReceiverName, string OriginCity, string DestinationCity, 
decimal WeightKg, string Status, DateTime CreatedAt);
