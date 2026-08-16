using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();

    Task<Category?> GetByIdAsync(int id);

    Task<Category?> GetByNameAsync(string name);

    Task AddAsync(Category category);

    Task SaveChangesAsync();
}