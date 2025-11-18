using BookingService.Application.DTOs;

namespace BookingService.Application.Interfaces;

public interface IStationService
{
    Task<IEnumerable<StationSearchResponse>> SearchNearbyStationsAsync(StationSearchRequest request);
    Task<BatteryAvailabilityResponse> GetBatteryAvailabilityAsync(Guid stationId, Guid? batteryTypeId = null);
}
