using BookingService.Domain.Models;

namespace BookingService.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid vehicleId);
    Task<bool> BelongsToDriverAsync(Guid vehicleId, Guid driverId);
    Task<Guid?> GetBatteryTypeIdAsync(Guid vehicleId);
}
