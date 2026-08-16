using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task<Product?> GetByNameAsync(string name);

    Task AddAsync(Product product);
    Task<bool> ReduceStockAsync(
    int productId,
    int quantity);
    Task<bool> RestoreStockAsync(
    int productId,
    int quantity);
    Task SaveChangesAsync();
}