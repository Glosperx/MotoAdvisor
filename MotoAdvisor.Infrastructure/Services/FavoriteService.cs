using Microsoft.Extensions.Logging;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IUserFavoriteRepository _repo;
    private readonly IMotorcycleRepository _motorcycleRepo;
    private readonly AppDbContext _context;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(IUserFavoriteRepository repo, IMotorcycleRepository motorcycleRepo, AppDbContext context, ILogger<FavoriteService> logger)
    {
        _repo = repo;
        _motorcycleRepo = motorcycleRepo;
        _context = context;
        _logger = logger;
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
        if (await _repo.ExistsAsync(userId, motorcycleId))
        {
            _logger.LogWarning("Motorcycle {MotorcycleId} is already in favorites for user {UserId}", motorcycleId, userId);
            return false;
        }

        var motorcycle = await _motorcycleRepo.GetByIdAsync(motorcycleId);
        if (motorcycle is null)
        {
            _logger.LogWarning("Add favorite failed — motorcycle with id {MotorcycleId} not found", motorcycleId);
            return false;
        }

        await _repo.AddAsync(new UserFavorite { UserId = userId, MotorcycleId = motorcycleId });
        await _context.SaveChangesAsync();
        _logger.LogInformation("Motorcycle {MotorcycleId} added to favorites for user {UserId}", motorcycleId, userId);
        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, int motorcycleId)
    {
        var favorite = await _repo.GetAsync(userId, motorcycleId);
        if (favorite is null)
        {
            _logger.LogWarning("Remove favorite failed — motorcycle {MotorcycleId} not in favorites for user {UserId}", motorcycleId, userId);
            return false;
        }

        await _repo.RemoveAsync(favorite);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Motorcycle {MotorcycleId} removed from favorites for user {UserId}", motorcycleId, userId);
        return true;
    }
}
