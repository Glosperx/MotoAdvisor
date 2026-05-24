using MotoAdvisor.Core.Entities;

namespace MotoAdvisor.Core.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetByMotorcycleIdAsync(int motorcycleId);
    Task<Review?> GetByUserAndMotorcycleAsync(string userId, int motorcycleId);
}
