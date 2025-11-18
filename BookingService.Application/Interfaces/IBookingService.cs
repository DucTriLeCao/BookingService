using BookingService.Application.DTOs;

namespace BookingService.Application.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, Guid driverId);
    Task<IEnumerable<BookingResponse>> GetDriverBookingsAsync(Guid driverId);
    Task<BookingResponse> GetBookingByIdAsync(Guid bookingId, Guid driverId);
    Task<BookingResponse> CancelBookingAsync(Guid bookingId, Guid driverId);
}
