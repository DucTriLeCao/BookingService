using BookingService.Domain.Models;

namespace BookingService.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid bookingId);
    Task<IEnumerable<Booking>> GetByDriverIdAsync(Guid driverId);
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking> UpdateAsync(Booking booking);
    Task<bool> HasConflictingBookingAsync(Guid stationId, DateTime scheduledTime);
}
