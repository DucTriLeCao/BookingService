using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "driver")]
public class StationsController : ControllerBase
{
    private readonly IStationService _stationService;

    public StationsController(IStationService stationService)
    {
        _stationService = stationService;
    }

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<StationSearchResponse>>> SearchNearbyStations(
        [FromBody] StationSearchRequest request)
    {
        try
        {
            var stations = await _stationService.SearchNearbyStationsAsync(request);
            return Ok(stations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{stationId}/battery-availability")]
    public async Task<ActionResult<BatteryAvailabilityResponse>> GetBatteryAvailability(
        Guid stationId,
        [FromQuery] Guid? batteryTypeId = null)
    {
        try
        {
            var availability = await _stationService.GetBatteryAvailabilityAsync(stationId, batteryTypeId);
            return Ok(availability);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
