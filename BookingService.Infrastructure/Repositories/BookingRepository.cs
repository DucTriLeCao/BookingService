using BookingService.Domain.Interfaces;
using BookingService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ev_battery_swapContext _context;

    public BookingRepository(ev_battery_swapContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid bookingId)
    {
        return await _context.Bookings
            .Include(b => b.Station)
            .Include(b => b.Vehicle)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task<IEnumerable<Booking>> GetByDriverIdAsync(Guid driverId)
    {
        return await _context.Bookings
            .Include(b => b.Station)
            .Include(b => b.Vehicle)
            .Where(b => b.DriverId == driverId)
            .OrderByDescending(b => b.ScheduledTime)
            .ToListAsync();
    }

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking> UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<bool> HasConflictingBookingAsync(Guid stationId, DateTime scheduledTime)
    {
        var timeWindow = TimeSpan.FromMinutes(15);
        var startTime = scheduledTime.Subtract(timeWindow);
        var endTime = scheduledTime.Add(timeWindow);

        return await _context.Bookings
            .AnyAsync(b => b.StationId == stationId &&
                          b.Status == "confirmed" &&
                          b.ScheduledTime >= startTime &&
                          b.ScheduledTime <= endTime);
    }
}
