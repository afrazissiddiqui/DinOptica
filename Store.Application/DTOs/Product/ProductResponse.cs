namespace Store.Application.DTOs.Product;

public class ProductResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
}