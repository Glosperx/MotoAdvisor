using MotoAdvisor.Core.DTOs;

namespace MotoAdvisor.Core.Interfaces;

public interface IFavoriteService
{
    Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId);
    Task<bool> AddFavoriteAsync(string userId, int motorcycleId);
    Task<bool> RemoveFavoriteAsync(string userId, int motorcycleId);
}
