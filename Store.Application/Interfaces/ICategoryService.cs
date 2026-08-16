using Store.Application.DTOs.Category;

namespace Store.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse?> GetByIdAsync(int id);

    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
}