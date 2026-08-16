using Store.Application.DTOs.Cart;
using Store.Application.Interfaces;
using Store.Domain.Entities;

namespace Store.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartResponse> GetOrCreateCartAsync(int userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();

            cart = await _cartRepository.GetByUserIdAsync(userId);
        }

        return MapToResponse(cart!);
    }

    public async Task<CartResponse> AddItemAsync(
        int userId,
        AddToCartRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var product =
            await _productRepository.GetByIdAsync(request.ProductId);

        if (product is null || !product.IsActive)
        {
            throw new InvalidOperationException(
                "Product not found or inactive.");
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();

            cart = await _cartRepository.GetByUserIdAsync(userId);
        }

        var existingItem = cart!.Items
            .FirstOrDefault(x => x.ProductId == request.ProductId);

        var newQuantity = existingItem is null
            ? request.Quantity
            : existingItem.Quantity + request.Quantity;

        if (newQuantity > product.StockQuantity)
        {
            throw new InvalidOperationException(
                "Requested quantity exceeds available stock.");
        }

        if (existingItem is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });
        }
        else
        {
            existingItem.Quantity = newQuantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        var updatedCart =
            await _cartRepository.GetByUserIdAsync(userId);

        return MapToResponse(updatedCart!);
    }

    public async Task<CartResponse> UpdateItemAsync(
        int userId,
        int productId,
        UpdateCartItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            throw new InvalidOperationException(
                "Cart not found.");
        }

        var item = cart.Items
            .FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Product is not in the cart.");
        }

        var product =
            await _productRepository.GetByIdAsync(productId);

        if (product is null || !product.IsActive)
        {
            throw new InvalidOperationException(
                "Product not found or inactive.");
        }

        if (request.Quantity > product.StockQuantity)
        {
            throw new InvalidOperationException(
                "Requested quantity exceeds available stock.");
        }

        item.Quantity = request.Quantity;

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        var updatedCart =
            await _cartRepository.GetByUserIdAsync(userId);

        return MapToResponse(updatedCart!);
    }

    public async Task<CartResponse> RemoveItemAsync(
        int userId,
        int productId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            throw new InvalidOperationException(
                "Cart not found.");
        }

        var item = cart.Items
            .FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Product is not in the cart.");
        }

        await _cartRepository.RemoveItemAsync(item);

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        var updatedCart =
            await _cartRepository.GetByUserIdAsync(userId);

        return MapToResponse(updatedCart!);
    }

    public async Task<CartResponse> GetOrCreateGuestCartAsync(
        string guestCartId)
    {
        if (string.IsNullOrWhiteSpace(guestCartId))
        {
            throw new InvalidOperationException(
                "Guest cart ID is required.");
        }

        var cart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        if (cart is null)
        {
            cart = new Cart
            {
                GuestCartId = guestCartId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();

            cart =
                await _cartRepository.GetByGuestCartIdAsync(guestCartId);
        }

        return MapToResponse(cart!);
    }

    public async Task<CartResponse> AddGuestItemAsync(
        string guestCartId,
        AddToCartRequest request)
    {
        if (string.IsNullOrWhiteSpace(guestCartId))
        {
            throw new InvalidOperationException(
                "Guest cart ID is required.");
        }

        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var product =
            await _productRepository.GetByIdAsync(request.ProductId);

        if (product is null || !product.IsActive)
        {
            throw new InvalidOperationException(
                "Product not found or inactive.");
        }

        var cart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        if (cart is null)
        {
            cart = new Cart
            {
                GuestCartId = guestCartId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();

            cart =
                await _cartRepository.GetByGuestCartIdAsync(guestCartId);
        }

        var existingItem = cart!.Items
            .FirstOrDefault(x => x.ProductId == request.ProductId);

        var newQuantity = existingItem is null
            ? request.Quantity
            : existingItem.Quantity + request.Quantity;

        if (newQuantity > product.StockQuantity)
        {
            throw new InvalidOperationException(
                "Requested quantity exceeds available stock.");
        }

        if (existingItem is null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            });
        }
        else
        {
            existingItem.Quantity = newQuantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        var updatedCart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        return MapToResponse(updatedCart!);
    }

    public async Task<CartResponse> UpdateGuestItemAsync(
        string guestCartId,
        int productId,
        UpdateCartItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(guestCartId))
        {
            throw new InvalidOperationException(
                "Guest cart ID is required.");
        }

        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var cart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        if (cart is null)
        {
            throw new InvalidOperationException(
                "Cart not found.");
        }

        var item = cart.Items
            .FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Product is not in the cart.");
        }

        var product =
            await _productRepository.GetByIdAsync(productId);

        if (product is null || !product.IsActive)
        {
            throw new InvalidOperationException(
                "Product not found or inactive.");
        }

        if (request.Quantity > product.StockQuantity)
        {
            throw new InvalidOperationException(
                "Requested quantity exceeds available stock.");
        }

        item.Quantity = request.Quantity;

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        var updatedCart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        return MapToResponse(updatedCart!);
    }

    public async Task<CartResponse> RemoveGuestItemAsync(
        string guestCartId,
        int productId)
    {
        if (string.IsNullOrWhiteSpace(guestCartId))
        {
            throw new InvalidOperationException(
                "Guest cart ID is required.");
        }

        var cart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        if (cart is null)
        {
            throw new InvalidOperationException(
                "Cart not found.");
        }

        var item = cart.Items
            .FirstOrDefault(x => x.ProductId == productId);

        if (item is null)
        {
            throw new InvalidOperationException(
                "Product is not in the cart.");
        }

        await _cartRepository.RemoveItemAsync(item);

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync();

        var updatedCart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        return MapToResponse(updatedCart!);
    }
    public async Task MergeGuestCartAsync(
    string guestCartId,
    int userId)
    {
        if (string.IsNullOrWhiteSpace(guestCartId))
        {
            return;
        }

        var guestCart =
            await _cartRepository.GetByGuestCartIdAsync(guestCartId);

        if (guestCart is null)
        {
            return;
        }

        var userCart =
            await _cartRepository.GetByUserIdAsync(userId);

        if (userCart is null)
        {
            guestCart.UserId = userId;
            guestCart.GuestCartId = null;
            guestCart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.SaveChangesAsync();

            return;
        }

        await _cartRepository.MergeGuestCartAsync(
            guestCart,
            userCart);

        await _cartRepository.SaveChangesAsync();
    }
    private static CartResponse MapToResponse(Cart cart)
    {
        var items = cart.Items.Select(item => new CartItemResponse
        {
            ProductId = item.ProductId,
            ProductName = item.Product.Name,
            UnitPrice = item.Product.Price,
            Quantity = item.Quantity,
            TotalPrice = item.Product.Price * item.Quantity,
            ImageUrl = item.Product.ImageUrl
        }).ToList();

        return new CartResponse
        {
            Id = cart.Id,
            Items = items,
            TotalAmount = items.Sum(x => x.TotalPrice)
        };
    }
}