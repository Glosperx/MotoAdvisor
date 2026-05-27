using Microsoft.Extensions.Logging;
using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly AppDbContext _context;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(ICategoryRepository repo, AppDbContext context, ILogger<CategoryService> logger)
    {
        _repo = repo;
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(Map);

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null)
        {
            _logger.LogWarning("Category with id {Id} not found", id);
            return null;
        }
        return Map(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        await _repo.AddAsync(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Category created with id {Id} and name '{Name}'", category.Id, category.Name);
        return Map(category);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null)
        {
            _logger.LogWarning("Update failed — category with id {Id} not found", id);
            return false;
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        await _repo.UpdateAsync(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Category with id {Id} updated", id);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _repo.GetByIdAsync(id);
        if (category is null)
        {
            _logger.LogWarning("Delete failed — category with id {Id} not found", id);
            return false;
        }
        await _repo.DeleteAsync(category);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Category with id {Id} deleted", id);
        return true;
    }

    private static CategoryDto Map(Category c) => new(c.Id, c.Name, c.Description);
}
