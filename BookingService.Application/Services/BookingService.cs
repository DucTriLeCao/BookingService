using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using BookingService.Domain.Interfaces;
using BookingService.Domain.Models;

namespace BookingService.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IStationRepository _stationRepository;
    private readonly IBatteryRepository _batteryRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public BookingService(
        IBookingRepository bookingRepository, 
        IStationRepository stationRepository,
        IBatteryRepository batteryRepository,
        IVehicleRepository vehicleRepository)
    {
        _bookingRepository = bookingRepository;
        _stationRepository = stationRepository;
        _batteryRepository = batteryRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, Guid driverId)
    {
        var vehicleBelongsToDriver = await _vehicleRepository.BelongsToDriverAsync(
            request.VehicleId, 
            driverId);
        
        if (!vehicleBelongsToDriver)
        {
            throw new UnauthorizedAccessException("Vehicle does not belong to this driver");
        }

        var station = await _stationRepository.GetByIdAsync(request.StationId);
        if (station == null)
        {
            throw new KeyNotFoundException($"Station with ID {request.StationId} not found");
        }

        if (station.Status != "active")
        {
            throw new InvalidOperationException("Station is not active");
        }

        if (request.ScheduledTime <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("Scheduled time must be in the future");
        }

        var hasConflict = await _bookingRepository.HasConflictingBookingAsync(
            request.StationId, 
            request.ScheduledTime);

        if (hasConflict)
        {
            throw new InvalidOperationException("Time slot is not available");
        }

        var vehicleBatteryTypeId = await _vehicleRepository.GetBatteryTypeIdAsync(request.VehicleId);
        if (!vehicleBatteryTypeId.HasValue)
        {
            throw new InvalidOperationException("Vehicle battery type not found");
        }

        var availableBatteries = await _batteryRepository.GetAvailableCountAsync(
            request.StationId, 
            vehicleBatteryTypeId.Value);
        
        if (availableBatteries == 0)
        {
            throw new InvalidOperationException("No compatible batteries available at this station for your vehicle type");
        }

        var booking = new Booking
        {
            BookingId = Guid.NewGuid(),
            DriverId = driverId,
            VehicleId = request.VehicleId,
            StationId = request.StationId,
            BookingTime = DateTime.UtcNow,
            ScheduledTime = request.ScheduledTime,
            Status = "pending",
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdBooking = await _bookingRepository.CreateAsync(booking);

        return new BookingResponse
        {
            BookingId = createdBooking.BookingId,
            DriverId = createdBooking.DriverId,
            VehicleId = createdBooking.VehicleId,
            StationId = createdBooking.StationId,
            StationName = station.StationName,
            StationAddress = station.Address,
            BookingTime = createdBooking.BookingTime,
            ScheduledTime = createdBooking.ScheduledTime,
            Status = createdBooking.Status,
            Notes = createdBooking.Notes,
            CreatedAt = createdBooking.CreatedAt
        };
    }

    public async Task<IEnumerable<BookingResponse>> GetDriverBookingsAsync(Guid driverId)
    {
        var bookings = await _bookingRepository.GetByDriverIdAsync(driverId);

        return bookings.Select(b => new BookingResponse
        {
            BookingId = b.BookingId,
            DriverId = b.DriverId,
            VehicleId = b.VehicleId,
            StationId = b.StationId,
            StationName = b.Station?.StationName,
            StationAddress = b.Station?.Address,
            BookingTime = b.BookingTime,
            ScheduledTime = b.ScheduledTime,
            Status = b.Status,
            Notes = b.Notes,
            CreatedAt = b.CreatedAt
        });
    }

    public async Task<BookingResponse> GetBookingByIdAsync(Guid bookingId, Guid driverId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking with ID {bookingId} not found");
        }

        if (booking.DriverId != driverId)
        {
            throw new UnauthorizedAccessException("You can only view your own bookings");
        }

        return new BookingResponse
        {
            BookingId = booking.BookingId,
            DriverId = booking.DriverId,
            VehicleId = booking.VehicleId,
            StationId = booking.StationId,
            StationName = booking.Station?.StationName,
            StationAddress = booking.Station?.Address,
            BookingTime = booking.BookingTime,
            ScheduledTime = booking.ScheduledTime,
            Status = booking.Status,
            Notes = booking.Notes,
            CreatedAt = booking.CreatedAt
        };
    }

    public async Task<BookingResponse> CancelBookingAsync(Guid bookingId, Guid driverId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);
        if (booking == null)
        {
            throw new KeyNotFoundException($"Booking with ID {bookingId} not found");
        }

        if (booking.DriverId != driverId)
        {
            throw new UnauthorizedAccessException("You can only cancel your own bookings");
        }

        if (booking.Status == "completed" || booking.Status == "cancelled")
        {
            throw new InvalidOperationException($"Cannot cancel booking with status {booking.Status}");
        }

        booking.Status = "cancelled";
        booking.UpdatedAt = DateTime.UtcNow;

        var updatedBooking = await _bookingRepository.UpdateAsync(booking);

        return new BookingResponse
        {
            BookingId = updatedBooking.BookingId,
            DriverId = updatedBooking.DriverId,
            VehicleId = updatedBooking.VehicleId,
            StationId = updatedBooking.StationId,
            StationName = updatedBooking.Station?.StationName,
            StationAddress = updatedBooking.Station?.Address,
            BookingTime = updatedBooking.BookingTime,
            ScheduledTime = updatedBooking.ScheduledTime,
            Status = updatedBooking.Status,
            Notes = updatedBooking.Notes,
            CreatedAt = updatedBooking.CreatedAt
        };
    }
}
