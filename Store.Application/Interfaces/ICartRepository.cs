using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);

    Task<Cart?> GetByIdAsync(int cartId);

    Task AddAsync(Cart cart);

    Task<CartItem?> GetItemAsync(
    int cartId,
    int productId);
    Task ClearAsync(int cartId);
    Task RemoveItemAsync(CartItem item);

    Task SaveChangesAsync();
}