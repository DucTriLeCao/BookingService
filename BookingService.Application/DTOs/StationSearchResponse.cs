namespace BookingService.Application.DTOs;

public class StationSearchResponse
{
    public Guid StationId { get; set; }
    public string StationCode { get; set; }
    public string StationName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public double DistanceKm { get; set; }
    public int TotalSlots { get; set; }
    public int AvailableBatteries { get; set; }
    public string Status { get; set; }
    public bool Is24Hours { get; set; }
    public TimeOnly OperatingHoursStart { get; set; }
    public TimeOnly OperatingHoursEnd { get; set; }
}
