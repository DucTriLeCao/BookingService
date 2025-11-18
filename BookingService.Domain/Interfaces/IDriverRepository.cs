using BookingService.Domain.Models;

namespace BookingService.Domain.Interfaces;

public interface IDriverRepository
{
    Task<Driver?> GetByUserIdAsync(Guid userId);
    Task<Guid?> GetDriverIdByUserIdAsync(Guid userId);
}
