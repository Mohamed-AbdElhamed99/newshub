using NewsHub.Application.Admin.DTOs.Categories;
using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.Interfaces.Services;

public interface ICategoryAdminService
{
    Task<CategoryAdminDetailDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<CategoryAdminDetailDto?> GetCategoryByIdAsync(int id);
    Task<PagedResult<CategoryAdminDto>> GetCategoriesAsync(CategoryFilter filter);
    Task<CategoryAdminDetailDto?> UpdateCategoryAsync(UpdateCategoryDto dto);
    Task<bool> DeleteCategoryAsync(int id);
}