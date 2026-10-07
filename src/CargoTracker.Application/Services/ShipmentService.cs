using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using CargoTracker.Domain.Entities;
using CargoTracker.Domain.Enums;

namespace CargoTracker.Application.Services;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICacheService _cacheService;

    public ShipmentService(IShipmentRepository shipmentRepository, IUserRepository userRepository, ICacheService cacheService)
    {
        _shipmentRepository = shipmentRepository;
        _userRepository = userRepository;
        _cacheService = cacheService;
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
        var cacheKey = ShipmentCacheKey(trackingNumber);
        var response = await _cacheService.GetAsync<ShipmentResponse>(cacheKey);

        if (response is null)
        {
            // cache'te yoktu (cache miss), veritabanına git
            var shipment = await _shipmentRepository.GetByTrackingNumberAsync(trackingNumber);
            if (shipment is null)
                return null;

            response = MapToResponse(shipment);
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        }

        // sahiplik kontrolü, cache'ten gelse de veritabanından gelse de aynı şekilde uygulanıyor
        if (requestingCustomerId.HasValue && response.CustomerId != requestingCustomerId.Value)
            return null;

        return response;
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

        await _cacheService.RemoveAsync(ShipmentCacheKey(trackingNumber));

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

        await _cacheService.RemoveAsync(ShipmentCacheKey(trackingNumber));

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
    private static string ShipmentCacheKey(string trackingNumber) => $"shipment:{trackingNumber}";
}
