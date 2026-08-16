using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);

    Task<Order?> GetByIdAsync(
        int orderId,
        int userId);

    Task<List<Order>> GetByUserIdAsync(
        int userId);

    Task SaveChangesAsync();
    Task<IDisposable> BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}