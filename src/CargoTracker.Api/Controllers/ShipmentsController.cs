using System.Security.Claims;
using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    private readonly IValidator<CreateShipmentRequest> _createShipmentValidator;

    public ShipmentsController(IShipmentService shipmentService, IValidator<CreateShipmentRequest> createShipmentValidator)
    {
        _shipmentService = shipmentService;
        _createShipmentValidator = createShipmentValidator;
    }


    /// <summary>Yeni bir kargo oluşturur.</summary>
    /// <remarks>Customer olarak oluşturulursa kargo otomatik olarak o müşteriye atanır.</remarks>
    [HttpPost]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<ActionResult<ShipmentResponse>> Create(CreateShipmentRequest request)
    {
        var validationResult = await _createShipmentValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return BadRequest(new ValidationProblemDetails(validationResult.ToDictionary()));

        Guid? customerId = User.IsInRole("Customer") ? GetCurrentUserId() : null;

        var response = await _shipmentService.CreateAsync(request, customerId);

        return CreatedAtAction(nameof(GetByTrackingNumber), new { trackingNumber = response.TrackingNumber }, response);
    }


    /// <summary>Takip numarasıyla tek bir kargoyu getirir.</summary>
    /// <remarks>Customer sadece kendi kargosunu görebilir; başkasının kargosu için 404 döner.</remarks>
    [HttpGet("{trackingNumber}")]
    public async Task<ActionResult<ShipmentResponse>> GetByTrackingNumber(string trackingNumber)
    {
        Guid? requestingCustomerId = User.IsInRole("Customer") ? GetCurrentUserId() : null;

        var response = await _shipmentService.GetByTrackingNumberAsync(trackingNumber, requestingCustomerId);

        return response is null ? NotFound() : Ok(response);
    }

    /// <summary>Tüm kargoları listeler.</summary>
    /// <remarks>Sadece Admin rolü erişebilir. Durum/şehir/tarihe göre filtrelenebilir, sayfalanabilir.</remarks>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PagedResult<ShipmentResponse>>> GetAll([FromQuery] ShipmentQuery query)
    {
        return Ok(await _shipmentService.GetAllAsync(query));
    }


    /// <summary>Giriş yapan müşterinin kendi kargolarını listeler.</summary>
    [HttpGet("mine")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<PagedResult<ShipmentResponse>>> GetMine([FromQuery] ShipmentQuery query)
    {
        return Ok(await _shipmentService.GetAllAsync(query));
    }

    /// <summary>Bir kargonun durumunu ilerletir.</summary>
    /// <remarks>Geçişler durum makinesine göre doğrulanır. Courier sadece kendine atanan kargoları güncelleyebilir.</remarks>
    [HttpPatch("{trackingNumber}/status")]
    [Authorize(Roles = "Courier,Admin")]
    public async Task<ActionResult<ShipmentResponse>> UpdateStatus(string trackingNumber, UpdateShipmentStatusRequest request)
    {
        Guid? requestingCourierId = User.IsInRole("Courier") ? GetCurrentUserId() : null;

        var response = await _shipmentService.UpdateStatusAsync(trackingNumber, request.NewStatus, requestingCourierId);
        if (response is null)
            return NotFound();
        return Ok(response);
    }

    /// <summary>Bir kargoya kurye atar.</summary>
    /// <remarks>Sadece Admin rolü erişebilir. Teslim edilmiş/iade edilmiş kargolara kurye atanamaz.</remarks>
    [HttpPatch("{trackingNumber}/courier")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ShipmentResponse>> AssignCourier(string trackingNumber, AssignCourierRequest request)
    {
        var response = await _shipmentService.AssignCourierAsync(trackingNumber, request.CourierId);
        if (response is null)
            return NotFound();
        return Ok(response);
    }

    /// <summary>Giriş yapan kuryeye atanmış kargoları listeler.</summary>
    [HttpGet("assigned")]
    [Authorize(Roles = "Courier")]
    public async Task<ActionResult<PagedResult<ShipmentResponse>>> GetAssigned([FromQuery] ShipmentQuery query)
    {
        return Ok(await _shipmentService.GetAllAsync(query));
    }

    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue("sub")!);
    }
}