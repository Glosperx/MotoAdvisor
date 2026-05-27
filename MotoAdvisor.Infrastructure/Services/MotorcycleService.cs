using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class MotorcycleService : IMotorcycleService
{
    private readonly IMotorcycleRepository _repo;
    private readonly AppDbContext _context;

    public MotorcycleService(IMotorcycleRepository repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<MotorcycleSummaryDto>> GetAllAsync(
        int? brandId = null, int? categoryId = null,
        decimal? minPrice = null, decimal? maxPrice = null)
    {
        var motorcycles = await _repo.GetAllWithDetailsAsync();

        if (brandId.HasValue)    motorcycles = motorcycles.Where(m => m.BrandId == brandId.Value);
        if (categoryId.HasValue) motorcycles = motorcycles.Where(m => m.CategoryId == categoryId.Value);
        if (minPrice.HasValue)   motorcycles = motorcycles.Where(m => m.Price >= minPrice.Value);
        if (maxPrice.HasValue)   motorcycles = motorcycles.Where(m => m.Price <= maxPrice.Value);

        return motorcycles.Select(MapToSummary);
    }

    public async Task<MotorcycleDetailDto?> GetByIdAsync(int id)
    {
        var m = await _repo.GetWithDetailsAsync(id);
        return m is null ? null : MapToDetail(m);
    }

    public async Task<IEnumerable<MotorcycleSummaryDto>> SearchAsync(string query) =>
        (await _repo.SearchAsync(query)).Select(MapToSummary);

    public async Task<MotorcycleSummaryDto> CreateAsync(CreateMotorcycleDto dto)
    {
        var motorcycle = new Motorcycle
        {
            Name = dto.Name,
            Year = dto.Year,
            Price = dto.Price,
            Engine = dto.Engine,
            Power = dto.Power,
            Horsepower = dto.Horsepower,
            LicenseCategory = dto.LicenseCategory,
            IsBeginnerFriendly = dto.IsBeginnerFriendly,
            Description = dto.Description,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId
        };
        await _repo.AddAsync(motorcycle);
        await _context.SaveChangesAsync();

        var created = await _repo.GetWithDetailsAsync(motorcycle.Id);
        return MapToSummary(created!);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMotorcycleDto dto)
    {
        var motorcycle = await _repo.GetByIdAsync(id);
        if (motorcycle is null) return false;

        motorcycle.Name = dto.Name;
        motorcycle.Year = dto.Year;
        motorcycle.Price = dto.Price;
        motorcycle.Engine = dto.Engine;
        motorcycle.Power = dto.Power;
        motorcycle.Horsepower = dto.Horsepower;
        motorcycle.LicenseCategory = dto.LicenseCategory;
        motorcycle.IsBeginnerFriendly = dto.IsBeginnerFriendly;
        motorcycle.Description = dto.Description;
        motorcycle.BrandId = dto.BrandId;
        motorcycle.CategoryId = dto.CategoryId;

        await _repo.UpdateAsync(motorcycle);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var motorcycle = await _repo.GetByIdAsync(id);
        if (motorcycle is null) return false;
        await _repo.DeleteAsync(motorcycle);
        await _context.SaveChangesAsync();
        return true;
    }

    private static MotorcycleSummaryDto MapToSummary(Motorcycle m) => new(
        m.Id, m.Name, m.Year, m.Price, m.Engine, m.Power, m.Horsepower,
        m.LicenseCategory, m.IsBeginnerFriendly,
        m.BrandId, m.Brand?.Name ?? string.Empty,
        m.CategoryId, m.Category?.Name ?? string.Empty,
        RealImageUrl(m)
    );

    // Returns the best real image URL (IsMain preferred, then any), ignoring
    // placehold.co seeder URLs so the frontend falls back to its own placeholder.
    private static string? RealImageUrl(Motorcycle m)
    {
        static bool IsReal(MotorcycleImage i) => !i.ImageUrl.Contains("placehold.co");
        return m.Images.FirstOrDefault(i => i.IsMain && IsReal(i))?.ImageUrl
            ?? m.Images.FirstOrDefault(IsReal)?.ImageUrl;
    }

    private static MotorcycleDetailDto MapToDetail(Motorcycle m) => new(
        m.Id, m.Name, m.Year, m.Price, m.Engine, m.Power, m.Horsepower,
        m.LicenseCategory, m.IsBeginnerFriendly, m.Description,
        new BrandDto(m.Brand.Id, m.Brand.Name, m.Brand.Country, m.Brand.LogoUrl),
        new CategoryDto(m.Category.Id, m.Category.Name, m.Category.Description),
        m.Images.Select(i => i.ImageUrl).ToList(),
        m.Reviews.Count > 0 ? m.Reviews.Average(r => r.Rating) : null,
        m.Reviews.Count
    );
}
