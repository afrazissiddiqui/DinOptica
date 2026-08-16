using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Entities;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly StoreDbContext _context;

    public CategoryRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Name == name);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}