using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) => _context = context;

    public async Task<Category?> GetByIdAsync(int id) =>
        await _context.Categories.FindAsync(id);

    public async Task<IEnumerable<Category>> GetAllAsync() =>
        await _context.Categories.ToListAsync();

    public async Task<Category> AddAsync(Category entity)
    {
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Category entity)
    {
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category entity)
    {
        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<Category?> GetByNameAsync(string name) =>
        await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
}
