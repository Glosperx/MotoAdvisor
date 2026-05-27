using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context) => _context = context;

    public async Task<Review?> GetByIdAsync(int id) =>
        await _context.Reviews.FindAsync(id);

    public async Task<IEnumerable<Review>> GetAllAsync() =>
        await _context.Reviews.Include(r => r.User).ToListAsync();

    public async Task<IEnumerable<Review>> GetByMotorcycleIdAsync(int motorcycleId) =>
        await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.MotorcycleId == motorcycleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task<Review?> GetByUserAndMotorcycleAsync(string userId, int motorcycleId) =>
        await _context.Reviews
            .FirstOrDefaultAsync(r => r.UserId == userId && r.MotorcycleId == motorcycleId);

    public Task<Review> AddAsync(Review entity)
    {
        _context.Reviews.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(Review entity)
    {
        _context.Reviews.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Review entity)
    {
        _context.Reviews.Remove(entity);
        return Task.CompletedTask;
    }
}
