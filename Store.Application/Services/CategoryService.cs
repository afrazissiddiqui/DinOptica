using Store.Application.DTOs.Category;
using Store.Application.Interfaces;
using Store.Domain.Entities;

namespace Store.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories.Select(MapToResponse).ToList();
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        return category is null
            ? null
            : MapToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request)
    {
        var name = request.Name.Trim();

        var existingCategory =
            await _categoryRepository.GetByNameAsync(name);

        if (existingCategory is not null)
        {
            throw new InvalidOperationException(
                "A category with this name already exists.");
        }

        var category = new Category
        {
            Name = name,
            Description = request.Description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return MapToResponse(category);
    }

    private static CategoryResponse MapToResponse(Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt
        };
    }
}