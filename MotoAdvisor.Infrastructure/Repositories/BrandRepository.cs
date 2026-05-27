using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context) => _context = context;

    public async Task<Brand?> GetByIdAsync(int id) =>
        await _context.Brands.FindAsync(id);

    public async Task<IEnumerable<Brand>> GetAllAsync() =>
        await _context.Brands.ToListAsync();

    public Task<Brand> AddAsync(Brand entity)
    {
        _context.Brands.Add(entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(Brand entity)
    {
        _context.Brands.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Brand entity)
    {
        _context.Brands.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Brand?> GetByNameAsync(string name) =>
        await _context.Brands.FirstOrDefaultAsync(b => b.Name == name);
}
