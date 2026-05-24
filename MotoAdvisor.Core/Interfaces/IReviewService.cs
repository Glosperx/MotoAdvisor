using MotoAdvisor.Core.DTOs;

namespace MotoAdvisor.Core.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetByMotorcycleAsync(int motorcycleId);
    Task<ReviewDto> CreateAsync(int motorcycleId, string userId, string userName, CreateReviewDto dto);
    Task<bool> UpdateAsync(int id, string userId, UpdateReviewDto dto);
    Task<ReviewDeleteResult> DeleteAsync(int id, string userId, bool isAdmin);
}
