using Store.Application.DTOs.Order;

namespace Store.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(
    int userId,
    CreateOrderRequest request);

    Task<OrderResponse?> GetByIdAsync(
        int orderId,
        int userId);

    Task<List<OrderResponse>> GetMyOrdersAsync(
        int userId);
    Task<OrderResponse?> CancelOrderAsync(
    int orderId,
    int userId);
}