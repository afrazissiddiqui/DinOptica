using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Entities;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreDbContext _context;

    public ProductRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Where(x => x.IsActive)
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }
    public async Task<bool> ReduceStockAsync(
    int productId,
    int quantity)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == productId);

        if (product is null)
        {
            return false;
        }

        if (product.StockQuantity < quantity)
        {
            return false;
        }

        product.StockQuantity -= quantity;

        return true;
    }
    public async Task<bool> RestoreStockAsync(
    int productId,
    int quantity)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == productId);

        if (product is null)
        {
            return false;
        }

        product.StockQuantity += quantity;

        return true;
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}