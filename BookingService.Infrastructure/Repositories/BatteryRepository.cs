using BookingService.Domain.Interfaces;
using BookingService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class BatteryRepository : IBatteryRepository
{
    private readonly ev_battery_swapContext _context;

    public BatteryRepository(ev_battery_swapContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Battery>> GetAvailableBatteriesAsync(Guid stationId, Guid? batteryTypeId = null)
    {
        var query = _context.Batteries
            .Where(b => b.StationId == stationId && 
                       b.Status == "available" && 
                       b.ChargeLevel >= 80 &&
                       b.SohPercentage >= 70);

        if (batteryTypeId.HasValue)
        {
            query = query.Where(b => b.BatteryTypeId == batteryTypeId.Value);
        }

        return await query
            .OrderByDescending(b => b.ChargeLevel)
            .ThenByDescending(b => b.SohPercentage)
            .ToListAsync();
    }

    public async Task<int> GetAvailableCountAsync(Guid stationId, Guid? batteryTypeId = null)
    {
        var query = _context.Batteries
            .Where(b => b.StationId == stationId && 
                       b.Status == "available" && 
                       b.ChargeLevel >= 80 &&
                       b.SohPercentage >= 70);

        if (batteryTypeId.HasValue)
        {
            query = query.Where(b => b.BatteryTypeId == batteryTypeId.Value);
        }

        return await query.CountAsync();
    }
}
