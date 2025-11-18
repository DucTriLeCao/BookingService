namespace BookingService.Application.DTOs;

public class BookingResponse
{
    public Guid BookingId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid StationId { get; set; }
    public string StationName { get; set; }
    public string StationAddress { get; set; }
    public DateTime BookingTime { get; set; }
    public DateTime ScheduledTime { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
