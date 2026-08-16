using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);

    Task<Cart?> GetByGuestCartIdAsync(string guestCartId);

    Task AddAsync(Cart cart);
    Task MergeGuestCartAsync(
    Cart guestCart,
    Cart userCart);
    Task AddItemAsync(CartItem item);

    Task RemoveItemAsync(CartItem item);

    Task SaveChangesAsync();

    Task ClearAsync(int cartId);
}