using Store.Application.DTOs.Order;
using Store.Application.Interfaces;
using Store.Domain.Entities;

namespace Store.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderResponse> CreateOrderAsync(
        int userId,
        CreateOrderRequest request)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null || !cart.Items.Any())
        {
            throw new InvalidOperationException(
                "Your cart is empty.");
        }

        await _orderRepository.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                ShippingAddress = request.ShippingAddress.Trim(),
                City = request.City.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                CreatedAt = DateTime.UtcNow
            };
            foreach (var cartItem in cart.Items)
            {
                var product = await _productRepository
                    .GetByIdAsync(cartItem.ProductId);

                if (product is null || !product.IsActive)
                {
                    throw new InvalidOperationException(
                        $"Product '{cartItem.ProductId}' is no longer available.");
                }

                if (cartItem.Quantity > product.StockQuantity)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock available for '{product.Name}'.");
                }

                var stockReduced =
                    await _productRepository.ReduceStockAsync(
                        product.Id,
                        cartItem.Quantity);

                if (!stockReduced)
                {
                    throw new InvalidOperationException(
                        $"Not enough stock available for '{product.Name}'.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = cartItem.Quantity,
                    TotalPrice = product.Price * cartItem.Quantity
                };

                order.Items.Add(orderItem);
            }

            order.TotalAmount =
                order.Items.Sum(x => x.TotalPrice);

            await _orderRepository.AddAsync(order);

            await _productRepository.SaveChangesAsync();

            await _cartRepository.ClearAsync(cart.Id);

            await _cartRepository.SaveChangesAsync();

            await _orderRepository.SaveChangesAsync();

            await _orderRepository.CommitTransactionAsync();

            return MapToResponse(order);
        }
        catch
        {
            await _orderRepository.RollbackTransactionAsync();

            throw;
        }
    }

    public async Task<OrderResponse?> GetByIdAsync(
        int orderId,
        int userId)
    {
        var order = await _orderRepository.GetByIdAsync(
            orderId,
            userId);

        if (order is null)
        {
            return null;
        }

        return MapToResponse(order);
    }

    public async Task<List<OrderResponse>> GetMyOrdersAsync(
        int userId)
    {
        var orders = await _orderRepository
            .GetByUserIdAsync(userId);

        return orders
            .Select(MapToResponse)
            .ToList();
    }
    public async Task<OrderResponse?> CancelOrderAsync(
    int orderId,
    int userId)
    {
        var order = await _orderRepository.GetByIdAsync(
            orderId,
            userId);

        if (order is null)
        {
            return null;
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be cancelled.");
        }

        await _orderRepository.BeginTransactionAsync();

        try
        {
            foreach (var item in order.Items)
            {
                var stockRestored =
                    await _productRepository.RestoreStockAsync(
                        item.ProductId,
                        item.Quantity);

                if (!stockRestored)
                {
                    throw new InvalidOperationException(
                        $"Product '{item.ProductId}' could not be found.");
                }
            }

            order.Status = OrderStatus.Cancelled;

            await _productRepository.SaveChangesAsync();

            await _orderRepository.SaveChangesAsync();

            await _orderRepository.CommitTransactionAsync();

            return MapToResponse(order);
        }
        catch
        {
            await _orderRepository.RollbackTransactionAsync();

            throw;
        }
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            ShippingAddress = order.ShippingAddress,
            City = order.City,
            PhoneNumber = order.PhoneNumber,
            Items = order.Items
                .Select(item => new OrderItemResponse
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                })
                .ToList()
        };
    }
}