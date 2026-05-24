using MotoAdvisor.Core.Entities;

namespace MotoAdvisor.Core.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}
