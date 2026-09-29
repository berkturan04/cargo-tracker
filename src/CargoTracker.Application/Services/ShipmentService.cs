using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;

namespace CargoTracker.Application.Services;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUserRepository _userRepository;

    public ShipmentService(IShipmentRepository shipmentRepository, IUserRepository userRepository)
    {
        _shipmentRepository = shipmentRepository;
        _userRepository = userRepository;
    }

    public async Task<ShipmentResponse> CreateAsync(CreateShipmentRequest request, Guid? customerId)
    {
        var trackingNumber = "TRK" + Random.Shared.NextInt64(1_000_000_000, 10_000_000_000);

        var shipment = new Shipment
        (
            trackingNumber,
            request.ReceiverName,
            request.OriginCity,
            request.DestinationCity,
            request.WeightKg,
            customerId
        );
        await _shipmentRepository.AddAsync(shipment);

        return MapToResponse(shipment);
    }

    public async Task<IReadOnlyList<ShipmentResponse>> GetAllAsync()
    {
        var shipments = await _shipmentRepository.GetAllAsync();
        return shipments.Select(MapToResponse).ToList();
    }

    public async Task<ShipmentResponse?> GetByTrackingNumberAsync(string trackingNumber, Guid? requestingCustomerId)
    {
        var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
        if (shipment is null)
            return null;
        if (requestingCustomerId.HasValue && shipment.CustomerId != requestingCustomerId.Value)
            return null;
        return MapToResponse(shipment);
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
            shipment.CreatedAt,
            shipment.CustomerId,
            shipment.CourierId
        );
    }

    public async Task<ShipmentResponse?> UpdateStatusAsync(string trackingNumber, string newStatus, Guid? requestingCourierId)
    {
        var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
        if (shipment is null)
            return null;

        if (requestingCourierId.HasValue && shipment.CourierId != requestingCourierId.Value)
            throw new UnauthorizedAccessException("Bu kargo size atanmamış.");

        if (!Enum.TryParse<ShipmentStatus>(newStatus, true, out var parsedStatus))
            throw new ArgumentException($"Geçersiz gönderi durumu: {newStatus}");

        shipment.AdvanceTo(parsedStatus);

        await _shipmentRepository.SaveChangesAsync();

        return MapToResponse(shipment);
    }

    public async Task<IReadOnlyList<ShipmentResponse>> GetMyShipmentsAsync(Guid customerId)
    {
        var shipments = await _shipmentRepository.GetByCustomerIdAsync(customerId);
        return shipments.Select(MapToResponse).ToList();
    }

    public async Task<ShipmentResponse?> AssignCourierAsync(string trackingNumber, Guid courierId)
    {
        var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
        if (shipment is null)
            return null;

        var courier = await _userRepository.GetByIdAsync(courierId);
        if (courier is null || courier.Role != UserRole.Courier)
            throw new ArgumentException("Geçerli bir kurye bulunamadı.");

        shipment.AssignCourier(courierId);

        await _shipmentRepository.SaveChangesAsync();

        return MapToResponse(shipment);
    }
}