using MotoAdvisor.Core.Entities;

namespace MotoAdvisor.Core.Interfaces;

public interface IBrandRepository : IRepository<Brand>
{
    Task<Brand?> GetByNameAsync(string name);
}
