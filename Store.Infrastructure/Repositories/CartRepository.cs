using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Entities;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly StoreDbContext _context;

    public CartRepository(StoreDbContext context)
    {
        _context = context;
    }
    public async Task AddItemAsync(CartItem item)
    {
        await _context.CartItems.AddAsync(item);
    }
    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<Cart?> GetByIdAsync(int cartId)
    {
        return await _context.Carts
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == cartId);
    }
    public async Task<Cart?> GetByGuestCartIdAsync(
     string guestCartId)
    {
        return await _context.Carts
            .Include(x => x.Items)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x =>
                x.GuestCartId == guestCartId);
    }
    public async Task AddAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    public async Task<CartItem?> GetItemAsync(
    int cartId,
    int productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(x =>
                x.CartId == cartId &&
                x.ProductId == productId);
    }
    public async Task ClearAsync(int cartId)
    {
        var items = await _context.CartItems
            .Where(x => x.CartId == cartId)
            .ToListAsync();

        _context.CartItems.RemoveRange(items);
    }

    public async Task RemoveItemAsync(CartItem item)
    {
        _context.CartItems.Remove(item);
    }
    public async Task MergeGuestCartAsync(
    Cart guestCart,
    Cart userCart)
    {
        foreach (var guestItem in guestCart.Items)
        {
            var existingItem = userCart.Items
                .FirstOrDefault(x =>
                    x.ProductId == guestItem.ProductId);

            if (existingItem is null)
            {
                userCart.Items.Add(new CartItem
                {
                    ProductId = guestItem.ProductId,
                    Quantity = guestItem.Quantity
                });
            }
            else
            {
                existingItem.Quantity += guestItem.Quantity;
            }
        }

        userCart.UpdatedAt = DateTime.UtcNow;

        _context.CartItems.RemoveRange(guestCart.Items);
        _context.Carts.Remove(guestCart);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}