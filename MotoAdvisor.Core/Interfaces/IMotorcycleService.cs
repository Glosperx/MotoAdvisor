using MotoAdvisor.Core.DTOs;

namespace MotoAdvisor.Core.Interfaces;

public interface IMotorcycleService
{
    Task<IEnumerable<MotorcycleSummaryDto>> GetAllAsync(
        int? brandId = null, int? categoryId = null,
        decimal? minPrice = null, decimal? maxPrice = null);
    Task<MotorcycleDetailDto?> GetByIdAsync(int id);
    Task<IEnumerable<MotorcycleSummaryDto>> SearchAsync(string query);
    Task<MotorcycleSummaryDto> CreateAsync(CreateMotorcycleDto dto);
    Task<bool> UpdateAsync(int id, UpdateMotorcycleDto dto);
    Task<bool> DeleteAsync(int id);
}
