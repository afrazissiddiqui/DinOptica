using Store.Domain.Entities;

namespace Store.Application.DTOs.Order;

public class OrderResponse
{
    public int Id { get; set; }

    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public List<OrderItemResponse> Items { get; set; } = new();
}