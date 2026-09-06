using NewsHub.Application.Site.DTOs.Categories;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryListItemDto>> GetAllAsync();
}