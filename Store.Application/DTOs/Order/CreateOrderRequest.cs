namespace Store.Application.DTOs.Order;

public class CreateOrderRequest
{
    public string ShippingAddress { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

}