using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;

namespace CargoTracker.Application.Services;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;

    public ShipmentService(IShipmentRepository shipmentRepository)
    {
        _shipmentRepository = shipmentRepository;
    }

    public async Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request)
    {
         var trackingNumber = "TRK" + Random.Shared.NextInt64(1_000_000_000, 10_000_000_000);

         var shipment = new Shipment
         (
             trackingNumber,
             request.ReceiverName,
             request.OriginCity,
             request.DestinationCity,
             request.WeightKg

         );
         await _shipmentRepository.AddAsync(shipment);

         return MapToResponse(shipment);
         
    }

    public async Task<IReadOnlyList<ShipmentResponse>> GetAllAsync()
    {
        var shipments = await _shipmentRepository.GetAllAsync();
        return shipments.Select(MapToResponse).ToList();
    }

    public async Task<ShipmentResponse?> GetByTrackingNumberAsync(string trackingNumber)
    {
        var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
        return shipment is null ? null : MapToResponse(shipment);
    }
    
    private static ShipmentResponse MapToResponse(Shipment shipment)
    {
        return new ShipmentResponse(
            shipment.Id,
            shipment.TrackingNumber,
            shipment.ReceiverName,
            shipment.OriginCity,
            shipment.DestinationCity,
            shipment.WeightKg,
            shipment.Status.ToString(),
            shipment.CreatedAt);
    }

    public async Task<ShipmentResponse?> UpdateStatusAsync(string trackingNumber, string newStatus)
    {
        var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
        if (shipment is null)
            return null;
        if(!Enum.TryParse<ShipmentStatus>(newStatus, true, out var parsedStatus))
            throw new ArgumentException($"Geçersiz gönderi durumu: {newStatus}");
        
        shipment.AdvanceTo(parsedStatus);
        
        await _shipmentRepository.SaveChangesAsync();

        return MapToResponse(shipment);
    }
}
