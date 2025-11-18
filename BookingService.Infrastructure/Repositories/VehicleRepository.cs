using BookingService.Domain.Interfaces;
using BookingService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ev_battery_swapContext _context;

    public VehicleRepository(ev_battery_swapContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid vehicleId)
    {
        return await _context.Vehicles
            .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
    }

    public async Task<bool> BelongsToDriverAsync(Guid vehicleId, Guid driverId)
    {
        return await _context.Vehicles
            .AnyAsync(v => v.VehicleId == vehicleId && v.DriverId == driverId);
    }

    public async Task<Guid?> GetBatteryTypeIdAsync(Guid vehicleId)
    {
        var vehicle = await _context.Vehicles
            .Where(v => v.VehicleId == vehicleId)
            .Join(_context.VehicleModels,
                v => v.ModelId,
                vm => vm.ModelId,
                (v, vm) => new { vm.BatteryTypeId })
            .FirstOrDefaultAsync();

        return vehicle?.BatteryTypeId;
    }
}
