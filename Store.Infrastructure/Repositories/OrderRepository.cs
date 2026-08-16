using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Entities;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreDbContext _context;

    public OrderRepository(StoreDbContext context)
    {
        _context = context;
    }
    public async Task<Order?> GetByIdForAdminAsync(int orderId)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }
    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(x => x.Items)
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task<Order?> GetByIdAsync(
        int orderId,
        int userId)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x =>
                x.Id == orderId &&
                x.UserId == userId);
    }

    public async Task<List<Order>> GetByUserIdAsync(
        int userId)
    {
        return await _context.Orders
            .Include(x => x.Items)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
    public async Task<IDisposable> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        var transaction = _context.Database.CurrentTransaction;

        if (transaction is not null)
        {
            await transaction.CommitAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        var transaction = _context.Database.CurrentTransaction;

        if (transaction is not null)
        {
            await transaction.RollbackAsync();
        }
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}