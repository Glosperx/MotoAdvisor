using MotoAdvisor.Core.Entities;

namespace MotoAdvisor.Core.Interfaces;

public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    Task<Motorcycle?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Motorcycle>> GetAllWithDetailsAsync();
    Task<IEnumerable<Motorcycle>> SearchAsync(string query);
    Task<IEnumerable<Motorcycle>> GetByBrandAsync(int brandId);
    Task<IEnumerable<Motorcycle>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Motorcycle>> GetAllWithEmbeddingsAsync();
}
