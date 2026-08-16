using Store.Application.DTOs.Product;
using Store.Application.Interfaces;
using Store.Domain.Entities;

namespace Store.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(MapToResponse).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        return product is null
            ? null
            : MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                "Product name is required.");
        }

        if (request.Price < 0)
        {
            throw new InvalidOperationException(
                "Product price cannot be negative.");
        }

        if (request.StockQuantity < 0)
        {
            throw new InvalidOperationException(
                "Stock quantity cannot be negative.");
        }

        var category =
            await _categoryRepository.GetByIdAsync(request.CategoryId);

        if (category is null)
        {
            throw new InvalidOperationException(
                "The specified category does not exist.");
        }

        if (!category.IsActive)
        {
            throw new InvalidOperationException(
                "The specified category is inactive.");
        }

        var existingProduct =
            await _productRepository.GetByNameAsync(name);

        if (existingProduct is not null)
        {
            throw new InvalidOperationException(
                "A product with this name already exists.");
        }

        var product = new Product
        {
            Name = name,
            Description = request.Description.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl)
                ? null
                : request.ImageUrl.Trim(),
            CategoryId = request.CategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        var createdProduct =
            await _productRepository.GetByIdAsync(product.Id);

        return MapToResponse(createdProduct!);
    }

    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty
        };
    }
}