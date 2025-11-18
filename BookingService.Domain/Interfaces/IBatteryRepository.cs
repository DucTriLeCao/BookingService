using BookingService.Domain.Models;

namespace BookingService.Domain.Interfaces;

public interface IBatteryRepository
{
    Task<IEnumerable<Battery>> GetAvailableBatteriesAsync(Guid stationId, Guid? batteryTypeId = null);
    Task<int> GetAvailableCountAsync(Guid stationId, Guid? batteryTypeId = null);
}
