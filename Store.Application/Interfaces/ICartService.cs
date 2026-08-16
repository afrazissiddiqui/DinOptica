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

    Task<CartResponse> GetOrCreateGuestCartAsync(
        string guestCartId);

    Task<CartResponse> AddGuestItemAsync(
        string guestCartId,
        AddToCartRequest request);
    Task MergeGuestCartAsync(
    string guestCartId,
    int userId);
    Task<CartResponse> UpdateGuestItemAsync(
        string guestCartId,
        int productId,
        UpdateCartItemRequest request);

    Task<CartResponse> RemoveGuestItemAsync(
        string guestCartId,
        int productId);
}