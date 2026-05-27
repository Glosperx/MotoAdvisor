using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly AppDbContext _context;

    public CategoryService(ICategoryRepository repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(Map);

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        return category is null ? null : Map(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        await _repo.AddAsync(category);
        await _context.SaveChangesAsync();
        return Map(category);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null) return false;

        category.Name = dto.Name;
        category.Description = dto.Description;
        await _repo.UpdateAsync(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null) return false;
        await _repo.DeleteAsync(category);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CategoryDto Map(Category c) => new(c.Id, c.Name, c.Description);
}
