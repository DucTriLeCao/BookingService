namespace BookingService.Application.DTOs;

public class CreateBookingRequest
{
    public Guid VehicleId { get; set; }
    public Guid StationId { get; set; }
    public DateTime ScheduledTime { get; set; }
    public string Notes { get; set; }
}
