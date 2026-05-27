using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Repositories;

public class MotorcycleRepository : IMotorcycleRepository
{
    private readonly AppDbContext _context;

    public MotorcycleRepository(AppDbContext context) => _context = context;

    public async Task<Motorcycle?> GetByIdAsync(int id) =>
        await _context.Motorcycles.FindAsync(id);

    public async Task<IEnumerable<Motorcycle>> GetAllAsync() =>
        await _context.Motorcycles.ToListAsync();

    public async Task<Motorcycle?> GetWithDetailsAsync(int id) =>
        await _context.Motorcycles
            .Include(m => m.Brand)
            .Include(m => m.Category)
            .Include(m => m.Images)
            .Include(m => m.Reviews).ThenInclude(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<IEnumerable<Motorcycle>> GetAllWithDetailsAsync() =>
        await _context.Motorcycles
            .Include(m => m.Brand)
            .Include(m => m.Category)
            .Include(m => m.Images.Where(i => i.IsMain))
            .ToListAsync();

    public async Task<IEnumerable<Motorcycle>> SearchAsync(string query) =>
        await _context.Motorcycles
            .Include(m => m.Brand)
            .Include(m => m.Category)
            .Where(m => m.Name.Contains(query) ||
                        m.Description!.Contains(query) ||
                        m.Brand.Name.Contains(query))
            .ToListAsync();

    public async Task<IEnumerable<Motorcycle>> GetByBrandAsync(int brandId) =>
        await _context.Motorcycles
            .Include(m => m.Category)
            .Where(m => m.BrandId == brandId)
            .ToListAsync();

    public async Task<IEnumerable<Motorcycle>> GetByCategoryAsync(int categoryId) =>
        await _context.Motorcycles
            .Include(m => m.Brand)
            .Where(m => m.CategoryId == categoryId)
            .ToListAsync();

    public async Task<IEnumerable<Motorcycle>> GetAllWithEmbeddingsAsync() =>
        await _context.Motorcycles
            .Where(m => m.EmbeddingVector != null)
            .ToListAsync();

    public Task<Motorcycle> AddAsync(Motorcycle entity)
    {
        _context.Motorcycles.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(Motorcycle entity)
    {
        _context.Motorcycles.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Motorcycle entity)
    {
        _context.Motorcycles.Remove(entity);
        return Task.CompletedTask;
    }
}
