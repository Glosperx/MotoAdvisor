using MotoAdvisor.Core.Entities;

namespace MotoAdvisor.Core.Interfaces;

public interface IUserFavoriteRepository
{
    Task<IEnumerable<UserFavorite>> GetByUserIdAsync(string userId);
    Task<UserFavorite?> GetAsync(string userId, int motorcycleId);
    Task AddAsync(UserFavorite favorite);
    Task RemoveAsync(UserFavorite favorite);
    Task<bool> ExistsAsync(string userId, int motorcycleId);
}
