using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;

namespace MotoAdvisor.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IUserFavoriteRepository _repo;
    private readonly IMotorcycleRepository _motorcycleRepo;

    public FavoriteService(IUserFavoriteRepository repo, IMotorcycleRepository motorcycleRepo)
    {
        _repo = repo;
        _motorcycleRepo = motorcycleRepo;
    }

    public async Task<IEnumerable<FavoriteDto>> GetUserFavoritesAsync(string userId) =>
        (await _repo.GetByUserIdAsync(userId)).Select(uf => new FavoriteDto(
            uf.MotorcycleId,
            uf.Motorcycle.Name,
            uf.Motorcycle.Year,
            uf.Motorcycle.Price,
            uf.Motorcycle.Brand?.Name ?? string.Empty,
            uf.Motorcycle.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl
        ));

    public async Task<bool> AddFavoriteAsync(string userId, int motorcycleId)
    {
        if (await _repo.ExistsAsync(userId, motorcycleId)) return false;

        var motorcycle = await _motorcycleRepo.GetByIdAsync(motorcycleId);
        if (motorcycle is null) return false;

        await _repo.AddAsync(new UserFavorite { UserId = userId, MotorcycleId = motorcycleId });
        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, int motorcycleId)
    {
        var favorite = await _repo.GetAsync(userId, motorcycleId);
        if (favorite is null) return false;

        await _repo.RemoveAsync(favorite);
        return true;
    }
}
