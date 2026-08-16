using Store.Application.DTOs.Cart;

namespace Store.Application.Interfaces;

public interface ICartService
{
    Task<CartResponse> GetOrCreateCartAsync(int userId);

    Task<CartResponse> AddItemAsync(
        int userId,
        AddToCartRequest request);
    Task<CartResponse> UpdateItemAsync(
    int userId,
    int productId,
    UpdateCartItemRequest request);
    Task<CartResponse> RemoveItemAsync(
    int userId,
    int productId);
    
}