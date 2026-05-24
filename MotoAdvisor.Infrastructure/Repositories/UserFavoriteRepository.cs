using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Repositories;

public class UserFavoriteRepository : IUserFavoriteRepository
{
    private readonly AppDbContext _context;

    public UserFavoriteRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<UserFavorite>> GetByUserIdAsync(string userId) =>
        await _context.UserFavorites
            .Include(uf => uf.Motorcycle).ThenInclude(m => m.Brand)
            .Include(uf => uf.Motorcycle).ThenInclude(m => m.Images.Where(i => i.IsMain))
            .Where(uf => uf.UserId == userId)
            .ToListAsync();

    public async Task<UserFavorite?> GetAsync(string userId, int motorcycleId) =>
        await _context.UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.MotorcycleId == motorcycleId);

    public async Task AddAsync(UserFavorite favorite)
    {
        _context.UserFavorites.Add(favorite);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(UserFavorite favorite)
    {
        _context.UserFavorites.Remove(favorite);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string userId, int motorcycleId) =>
        await _context.UserFavorites
            .AnyAsync(uf => uf.UserId == userId && uf.MotorcycleId == motorcycleId);
}
