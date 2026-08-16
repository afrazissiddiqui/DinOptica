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
    Task<Order?> GetByIdForAdminAsync(int orderId);
    Task CommitTransactionAsync();
    Task<List<Order>> GetAllAsync();
    Task RollbackTransactionAsync();
}