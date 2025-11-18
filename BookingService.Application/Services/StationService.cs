using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using BookingService.Domain.Interfaces;

namespace BookingService.Application.Services;

public class StationService : IStationService
{
    private readonly IStationRepository _stationRepository;
    private readonly IBatteryRepository _batteryRepository;

    public StationService(IStationRepository stationRepository, IBatteryRepository batteryRepository)
    {
        _stationRepository = stationRepository;
        _batteryRepository = batteryRepository;
    }

    public async Task<IEnumerable<StationSearchResponse>> SearchNearbyStationsAsync(StationSearchRequest request)
    {
        var stations = await _stationRepository.GetNearbyStationsAsync(
            request.Latitude, 
            request.Longitude, 
            request.RadiusKm);

        var responses = new List<StationSearchResponse>();

        foreach (var station in stations)
        {
            var availableBatteries = await _batteryRepository.GetAvailableCountAsync(
                station.StationId, 
                request.BatteryTypeId);

            var distance = CalculateDistance(
                request.Latitude, 
                request.Longitude, 
                station.Latitude, 
                station.Longitude);

            responses.Add(new StationSearchResponse
            {
                StationId = station.StationId,
                StationCode = station.StationCode,
                StationName = station.StationName,
                Address = station.Address,
                City = station.City,
                District = station.District,
                Latitude = station.Latitude,
                Longitude = station.Longitude,
                DistanceKm = distance,
                TotalSlots = station.TotalSlots,
                AvailableBatteries = availableBatteries,
                Status = station.Status,
                Is24Hours = station.Is24Hours,
                OperatingHoursStart = station.OperatingHoursStart,
                OperatingHoursEnd = station.OperatingHoursEnd
            });
        }

        return responses.OrderBy(r => r.DistanceKm);
    }

    public async Task<BatteryAvailabilityResponse> GetBatteryAvailabilityAsync(Guid stationId, Guid? batteryTypeId = null)
    {
        var station = await _stationRepository.GetByIdAsync(stationId);
        if (station == null)
        {
            throw new KeyNotFoundException($"Station with ID {stationId} not found");
        }

        var batteries = await _batteryRepository.GetAvailableBatteriesAsync(stationId, batteryTypeId);

        return new BatteryAvailabilityResponse
        {
            StationId = station.StationId,
            StationName = station.StationName,
            TotalAvailableBatteries = batteries.Count(),
            Batteries = batteries.Select(b => new BatteryInfo
            {
                BatteryId = b.BatteryId,
                BatteryCode = b.BatteryCode,
                ChargeLevel = b.ChargeLevel,
                SohPercentage = b.SohPercentage,
                Status = b.Status
            }).ToList()
        };
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
