using BookingService.Domain.Interfaces;
using BookingService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly ev_battery_swapContext _context;

    public DriverRepository(ev_battery_swapContext context)
    {
        _context = context;
    }

    public async Task<Driver?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Drivers
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<Guid?> GetDriverIdByUserIdAsync(Guid userId)
    {
        var driver = await _context.Drivers
            .Where(d => d.UserId == userId)
            .Select(d => d.DriverId)
            .FirstOrDefaultAsync();

        return driver == Guid.Empty ? null : driver;
    }
}
