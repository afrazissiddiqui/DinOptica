using Store.Application.DTOs.Admin;
using Store.Application.DTOs.Order;
using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface IAdminService
{
    Task<List<AdminUserResponse>> GetAllUsersAsync();
    Task<List<OrderResponse>> GetAllOrdersAsync();
    Task<OrderResponse?> UpdateOrderStatusAsync(
    int orderId,
    OrderStatus status);
}