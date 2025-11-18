using BookingService.Domain.Models;

namespace BookingService.Domain.Interfaces;

public interface IStationRepository
{
    Task<IEnumerable<Station>> GetNearbyStationsAsync(decimal latitude, decimal longitude, double radiusKm);
    Task<Station?> GetByIdAsync(Guid stationId);
    Task<int> GetAvailableBatteryCountAsync(Guid stationId, Guid? batteryTypeId = null);
}
