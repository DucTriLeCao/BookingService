using BookingService.Domain.Interfaces;
using BookingService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class StationRepository : IStationRepository
{
    private readonly ev_battery_swapContext _context;

    public StationRepository(ev_battery_swapContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Station>> GetNearbyStationsAsync(decimal latitude, decimal longitude, double radiusKm)
    {
        var stations = await _context.Stations
            .Where(s => s.Status == "active")
            .ToListAsync();

        var nearbyStations = stations
            .Select(s => new
            {
                Station = s,
                Distance = CalculateDistance(latitude, longitude, s.Latitude, s.Longitude)
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Select(x => x.Station)
            .ToList();

        return nearbyStations;
    }

    public async Task<Station?> GetByIdAsync(Guid stationId)
    {
        return await _context.Stations
            .FirstOrDefaultAsync(s => s.StationId == stationId);
    }

    public async Task<int> GetAvailableBatteryCountAsync(Guid stationId, Guid? batteryTypeId = null)
    {
        var query = _context.Batteries
            .Where(b => b.StationId == stationId && 
                       b.Status == "available" && 
                       b.ChargeLevel >= 80);

        if (batteryTypeId.HasValue)
        {
            query = query.Where(b => b.BatteryTypeId == batteryTypeId.Value);
        }

        return await query.CountAsync();
    }

    private static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
    {
        const double R = 6371;
        var dLat = ToRadians((double)(lat2 - lat1));
        var dLon = ToRadians((double)(lon2 - lon1));
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
