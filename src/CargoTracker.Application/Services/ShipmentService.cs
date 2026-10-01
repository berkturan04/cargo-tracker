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

    public Task<PagedResult<ShipmentResponse>> GetAllAsync(ShipmentQuery query)
    => GetPagedAsync(BuildFilter(query, customerId: null, courierId: null));

    public Task<PagedResult<ShipmentResponse>> GetMyShipmentsAsync(Guid customerId, ShipmentQuery query)
    => GetPagedAsync(BuildFilter(query, customerId, courierId: null));

    public Task<PagedResult<ShipmentResponse>> GetAssignedToMeAsync(Guid courierId, ShipmentQuery query)
        => GetPagedAsync(BuildFilter(query, customerId: null, courierId));

    private async Task<PagedResult<ShipmentResponse>> GetPagedAsync(ShipmentFilter filter)
    {
        var (shipments, totalCount) = await _shipmentRepository.GetFilteredAsync(filter);
        var responseItems = shipments.Select(MapToResponse).ToList();
        return new PagedResult<ShipmentResponse>(responseItems, filter.Page, filter.PageSize, totalCount);
    }

    private static ShipmentFilter BuildFilter(ShipmentQuery query, Guid? customerId, Guid? courierId)
    {
        if (query.Status != null && !Enum.TryParse<ShipmentStatus>(query.Status, true, out var parsedStatus))
            throw new ArgumentException($"Geçersiz gönderi durumu: {query.Status}");

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 || query.PageSize > 100 ? 10 : query.PageSize;

        return new ShipmentFilter
        {
            CustomerId = customerId,
            CourierId = courierId,
            Status = query.Status != null ? Enum.Parse<ShipmentStatus>(query.Status, true) : null,
            City = query.City,
            FromDate = query.FromDate,
            ToDate = query.ToDate,
            Page = page,
            PageSize = pageSize
        };
    }
}
