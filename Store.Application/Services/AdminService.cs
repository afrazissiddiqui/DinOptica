using Store.Application.DTOs.Admin;
using Store.Application.DTOs.Order;
using Store.Application.Interfaces;
using Store.Domain.Entities;

namespace Store.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;

    public AdminService(IUserRepository userRepository, IOrderRepository orderRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
    }
    public async Task<OrderResponse?> UpdateOrderStatusAsync(
    int orderId,
    OrderStatus status)
    {
        var order = await _orderRepository
            .GetByIdForAdminAsync(orderId);

        if (order is null)
        {
            return null;
        }

        if (!IsValidStatusTransition(
            order.Status,
            status))
        {
            throw new InvalidOperationException(
                $"Order cannot move from {order.Status} to {status}.");
        }

        order.Status = status;

        await _orderRepository.SaveChangesAsync();

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
    private static bool IsValidStatusTransition(
    OrderStatus currentStatus,
    OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending =>
                newStatus == OrderStatus.Confirmed ||
                newStatus == OrderStatus.Cancelled,

            OrderStatus.Confirmed =>
                newStatus == OrderStatus.Shipped ||
                newStatus == OrderStatus.Cancelled,

            OrderStatus.Shipped =>
                newStatus == OrderStatus.Delivered,

            OrderStatus.Delivered =>
                false,

            OrderStatus.Cancelled =>
                false,

            _ => false
        };
    }
    public async Task<List<OrderResponse>> GetAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders
            .Select(order => new OrderResponse
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
            })
            .ToList();
    }
    public async Task<List<AdminUserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new AdminUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        }).ToList();
    }
}