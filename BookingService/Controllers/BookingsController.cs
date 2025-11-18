using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "driver")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking(
        [FromBody] CreateBookingRequest request)
    {
        try
        {
            var driverId = Guid.Parse(User.FindFirst("driverId")?.Value 
                ?? throw new UnauthorizedAccessException("Driver ID not found in token"));
            
            var booking = await _bookingService.CreateBookingAsync(request, driverId);
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
            var driverId = Guid.Parse(User.FindFirst("driverId")?.Value 
                ?? throw new UnauthorizedAccessException("Driver ID not found in token"));
            
            var booking = await _bookingService.GetBookingByIdAsync(id, driverId);
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
            var driverId = Guid.Parse(User.FindFirst("driverId")?.Value 
                ?? throw new UnauthorizedAccessException("Driver ID not found in token"));
            
            var bookings = await _bookingService.GetDriverBookingsAsync(driverId);
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
            var driverId = Guid.Parse(User.FindFirst("driverId")?.Value 
                ?? throw new UnauthorizedAccessException("Driver ID not found in token"));
            
            var booking = await _bookingService.CancelBookingAsync(id, driverId);
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
