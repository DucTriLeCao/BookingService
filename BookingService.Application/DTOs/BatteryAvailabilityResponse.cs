namespace BookingService.Application.DTOs;

public class BatteryAvailabilityResponse
{
    public Guid StationId { get; set; }
    public string StationName { get; set; }
    public int TotalAvailableBatteries { get; set; }
    public List<BatteryInfo> Batteries { get; set; }
}

public class BatteryInfo
{
    public Guid BatteryId { get; set; }
    public string BatteryCode { get; set; }
    public int ChargeLevel { get; set; }
    public decimal SohPercentage { get; set; }
    public string Status { get; set; }
}
