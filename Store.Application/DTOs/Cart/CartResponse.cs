namespace Store.Application.DTOs.Cart;

public class CartResponse
{
    public int Id { get; set; }

    public List<CartItemResponse> Items { get; set; } = new();

    public decimal TotalAmount { get; set; }
}