namespace Store.Domain.Entities;

public class Cart
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? GuestCartId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public User? User { get; set; }

    public ICollection<CartItem> Items { get; set; } =
        new List<CartItem>();
}