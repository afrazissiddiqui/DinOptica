using Store.Application.DTOs.Product;

namespace Store.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync();

    Task<ProductResponse?> GetByIdAsync(int id);

    Task<ProductResponse> CreateAsync(CreateProductRequest request);
}