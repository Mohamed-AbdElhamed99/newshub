using System.Globalization;
using NewsHub.Application.Site.DTOs.Categories;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CategoryListItemDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(MapToListItemDto).ToList();
    }

    private static CategoryTranslation ResolveTranslation(Category category)
    {
        return category.Translations
                   .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
               ?? category.Translations.FirstOrDefault(t => t.LanguageCode == "en")
               ?? category.Translations.First();
    }

    private static CategoryListItemDto MapToListItemDto(Category category)
    {
        var translation = ResolveTranslation(category);

        return new CategoryListItemDto
        {
            Id = category.Id,
            Name = translation.Name,
            Description = translation.Description,
            ImagePath = category.ImagePath,
            CreatedAt = category.CreatedAt,
            Slug = translation.Slug,
            LanguageCode = translation.LanguageCode
        };
    }
}