using System.Security.Claims;
using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using CargoTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    [HttpPost]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<ActionResult<ShipmentResponse>> Create(CreateShipmentRequest request)
    {
        Guid? customerId = User.IsInRole("Customer") ? GetCurrentUserId() : null;

        var response = await _shipmentService.CreateAsync(request, customerId);

        return CreatedAtAction(nameof(GetByTrackingNumber), new { trackingNumber = response.TrackingNumber }, response);
    }

    [HttpGet("{trackingNumber}")]
    public async Task<ActionResult<ShipmentResponse>> GetByTrackingNumber(string trackingNumber)
    {
        Guid? requestingCustomerId = User.IsInRole("Customer") ? GetCurrentUserId() : null;

        var response = await _shipmentService.GetByTrackingNumberAsync(trackingNumber, requestingCustomerId);

        return response is null ? NotFound() : Ok(response);
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IReadOnlyList<ShipmentResponse>>> GetAll()
    {
        var list = await _shipmentService.GetAllAsync();

        return Ok(list);
    }

    [HttpGet("mine")]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult<IReadOnlyList<ShipmentResponse>>> GetMine()
    {
        var list = await _shipmentService.GetMyShipmentsAsync(GetCurrentUserId());
        return Ok(list);
    }

    [HttpPatch("{trackingNumber}/status")]
    [Authorize(Roles = "Courier,Admin")]
    public async Task<ActionResult<ShipmentResponse>> UpdateStatus(string trackingNumber, UpdateShipmentStatusRequest request)
    {
        try
        {
            var response = await _shipmentService.UpdateStatusAsync(trackingNumber, request.NewStatus);

            if (response is null)
                return NotFound();

            return Ok(response);
        }

        catch (ArgumentException)
        {
            return BadRequest("Geçersiz durum adı.");
        }

        catch (InvalidShipmentStatusTransitionException ex)
        {
            return BadRequest(ex.Message);
        }
    }
    private Guid GetCurrentUserId()
    {
        return Guid.Parse(User.FindFirstValue("sub")!);
    }
}
