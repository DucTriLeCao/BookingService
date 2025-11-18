namespace BookingService.Application.DTOs;

public class StationSearchRequest
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public double RadiusKm { get; set; } = 10;
    public Guid? BatteryTypeId { get; set; }
}
