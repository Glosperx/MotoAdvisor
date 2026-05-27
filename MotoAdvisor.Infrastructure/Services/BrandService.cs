using MotoAdvisor.Core.DTOs;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Core.Interfaces;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.Infrastructure.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repo;
    private readonly AppDbContext _context;

    public BrandService(IBrandRepository repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<IEnumerable<BrandDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(Map);

    public async Task<BrandDto?> GetByIdAsync(int id)
    {
        var brand = await _repo.GetByIdAsync(id);
        return brand is null ? null : Map(brand);
    }

    public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
    {
        var brand = new Brand { Name = dto.Name, Country = dto.Country, LogoUrl = dto.LogoUrl };
        await _repo.AddAsync(brand);
        await _context.SaveChangesAsync();
        return Map(brand);
    }

    public async Task<bool> UpdateAsync(int id, UpdateBrandDto dto)
    {
        var brand = await _repo.GetByIdAsync(id);
        if (brand is null) return false;

        brand.Name = dto.Name;
        brand.Country = dto.Country;
        brand.LogoUrl = dto.LogoUrl;
        await _repo.UpdateAsync(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var brand = await _repo.GetByIdAsync(id);
        if (brand is null) return false;
        await _repo.DeleteAsync(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    private static BrandDto Map(Brand b) => new(b.Id, b.Name, b.Country, b.LogoUrl);
}
