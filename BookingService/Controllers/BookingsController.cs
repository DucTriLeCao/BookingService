using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using BookingService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "driver")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IDriverRepository _driverRepository;

    public BookingsController(IBookingService bookingService, IDriverRepository driverRepository)
    {
        _bookingService = bookingService;
        _driverRepository = driverRepository;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking(
        [FromBody] CreateBookingRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");
            
            var userId = Guid.Parse(userIdClaim);
            var driverId = await _driverRepository.GetDriverIdByUserIdAsync(userId);
            
            if (!driverId.HasValue)
                throw new UnauthorizedAccessException("Driver profile not found for this user");
            
            var booking = await _bookingService.CreateBookingAsync(request, driverId.Value);
            return CreatedAtAction(nameof(GetBooking), new { id = booking.BookingId }, booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBooking(Guid id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");
            
            var userId = Guid.Parse(userIdClaim);
            var driverId = await _driverRepository.GetDriverIdByUserIdAsync(userId);
            
            if (!driverId.HasValue)
                throw new UnauthorizedAccessException("Driver profile not found for this user");
            
            var booking = await _bookingService.GetBookingByIdAsync(id, driverId.Value);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-bookings")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetDriverBookings()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");
            
            var userId = Guid.Parse(userIdClaim);
            var driverId = await _driverRepository.GetDriverIdByUserIdAsync(userId);
            
            if (!driverId.HasValue)
                throw new UnauthorizedAccessException("Driver profile not found for this user");
            
            var bookings = await _bookingService.GetDriverBookingsAsync(driverId.Value);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<BookingResponse>> CancelBooking(Guid id)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");
            
            var userId = Guid.Parse(userIdClaim);
            var driverId = await _driverRepository.GetDriverIdByUserIdAsync(userId);
            
            if (!driverId.HasValue)
                throw new UnauthorizedAccessException("Driver profile not found for this user");
            
            var booking = await _bookingService.CancelBookingAsync(id, driverId.Value);
            return Ok(booking);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
