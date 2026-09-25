using CargoTracker.Application.Abstractions;
using CargoTracker.Application.DTOs;
using CargoTracker.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CargoTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipmentService;

    public ShipmentsController(IShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

        [HttpPost]
        public async Task<ActionResult<ShipmentResponse>> Create(CreateShipmentRequest request)
        {
            var response = await _shipmentService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetByTrackingNumber), 
                new { trackingNumber = response.TrackingNumber }, 
                response
            );
        }

        [HttpGet("{trackingNumber}")]
        public async Task<ActionResult<ShipmentResponse>> GetByTrackingNumber(string trackingNumber)
        {
            var response = await _shipmentService.GetByTrackingNumberAsync(trackingNumber);

            return response is null ? NotFound() : Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ShipmentResponse>>> GetAll()
        {
            var list = await _shipmentService.GetAllAsync();

            return Ok(list);
        }
        [HttpPatch("{trackingNumber}/status")]
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
    }
