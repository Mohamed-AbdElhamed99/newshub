using System.Globalization;
using NewsHub.Application.Admin.DTOs.Categories;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Services;

public class CategoryAdminService : ICategoryAdminService
{
    private readonly ICategoryRepository _repository;

    public CategoryAdminService(ICategoryRepository repository)
    {
        this._repository = repository;
    }
    
    public async Task<CategoryAdminDetailDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            ImagePath = dto.ImagePath,
            Translations = dto.Translations.Select(t => new CategoryTranslation
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Slug = t.Slug,
                Description = t.Description
            }).ToList()
        };
        await _repository.AddAsync(category);
        return MapToDto(category);
    }

    public async Task<CategoryAdminDetailDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        return category == null ? null : MapToDto(category);
    }

    public async Task<PagedResult<CategoryAdminDto>> GetCategoriesAsync(CategoryFilter filter)
    {
        var (categories, totalCount) = await _repository.GetAllAsync(filter);

        var items = categories.Select(MapToListDto).ToList();

        return new PagedResult<CategoryAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<CategoryAdminDetailDto?> UpdateCategoryAsync(UpdateCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(dto.Id);
        if (category == null)  return null;
        
        category.ImagePath = dto.ImagePath;
        category.UpdatedAt = DateTime.UtcNow;

        foreach (var t in dto.Translations)
        {
            var existing = category.Translations.FirstOrDefault( e => e.LanguageCode == t.LanguageCode);
            if (existing != null)
            {
                existing.Name = t.Name;
                existing.Description = t.Description;
                existing.Slug = t.Slug;
            }
            else
            {
                category.Translations.Add(new CategoryTranslation
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Slug = t.Slug,
                    Description = t.Description
                });
            }
        }
        
        var incomingCodes = dto.Translations.Select(t => t.LanguageCode).ToHashSet();
        var toRemove = category.Translations.Where(t => !incomingCodes.Contains(t.LanguageCode)).ToList();
        foreach (var t in toRemove)
            category.Translations.Remove(t);
        
        await _repository.UpdateAsync(category);
        return MapToDto(category);
    }
    
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return false;

        await _repository.DeleteAsync(category);
        return true;
    }

    private static CategoryAdminDto MapToListDto(Category category)
    {
        var translation = category.Translations
            .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
              ?? category.Translations.FirstOrDefault(t => t.LanguageCode == "en")
              ?? category.Translations.First();
        
        return new CategoryAdminDto
        {
            Id = category.Id,
            ImagePath = category.ImagePath,
            Slug = translation.Slug,
            Name = translation.Name,
            Description = translation.Description,
            LanguageCode = translation.LanguageCode
        };
    }
    
    private static CategoryAdminDetailDto MapToDto(Category category) =>
        new CategoryAdminDetailDto
        {
            Id = category.Id,
            ImagePath = category.ImagePath,
            Translations = category.Translations.Select(t => new CategoryTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Slug = t.Slug,
                Description = t.Description
            }).ToList()
        }; 
}