namespace NewsHub.Application.Admin.Services;

using System.Globalization;
using NewsHub.Application.Admin.DTOs.Tags;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Common.Pagination;

using NewsHub.Domain.Entities;
public class TagAdminService : ITagAdminService
{
    private readonly ITagRepository _repository;

    public TagAdminService(ITagRepository repository)
    {
        this._repository = repository;
    }
    
    public async Task<TagAdminDetailDto> CreateTagAsync(CreateTagDto dto)
    {
        var tag = new Tag
        {
            Translations = dto.Translations.Select(t => new TagTranslation
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Slug = t.Slug
            }).ToList()
        };
        await _repository.AddAsync(tag);
        return MapToDto(tag);
    }

    public async Task<TagAdminDetailDto?> GetTagByIdAsync(int id)
    {
        var tag = await _repository.GetByIdAsync(id);
        return tag == null ? null : MapToDto(tag);
    }

    public async Task<PagedResult<TagAdminDto>> GetTagsAsync(TagFilter filter)
    {
        var (tags, totalCount) = await _repository.GetAllAsync(filter);

        var items = tags.Select(MapToListDto).ToList();

        return new PagedResult<TagAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<TagAdminDetailDto?> UpdateTagAsync(UpdateTagDto dto)
    {
        var tag = await _repository.GetByIdAsync(dto.Id);
        if (tag == null) return null;

        foreach (var t in dto.Translations)
        {
            var existing = tag.Translations.FirstOrDefault(e => e.LanguageCode == t.LanguageCode);
            if (existing != null)
            {
                existing.Name = t.Name;
                existing.Slug = t.Slug;
            }
            else
            {
                tag.Translations.Add(new TagTranslation
                {
                    LanguageCode = t.LanguageCode,
                    Name = t.Name,
                    Slug = t.Slug
                });
            }
        }

        var incomingCodes = dto.Translations.Select(t => t.LanguageCode).ToHashSet();
        var toRemove = tag.Translations.Where(t => !incomingCodes.Contains(t.LanguageCode)).ToList();
        foreach (var t in toRemove)
            tag.Translations.Remove(t);

        tag.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(tag);
        return MapToDto(tag);
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _repository.GetByIdAsync(id);
        if (tag == null) return false;

        await _repository.DeleteAsync(tag);
        return true;
    }

    private static TagAdminDto MapToListDto(Tag tag)
    {
        var translation = tag.Translations
            .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
              ?? tag.Translations.FirstOrDefault(t => t.LanguageCode == "en")
              ?? tag.Translations.First();

        return new TagAdminDto
        {
            Id = tag.Id,
            Name = translation.Name,
            Slug = translation.Slug,
            LanguageCode = translation.LanguageCode
        };
    }

    private static TagAdminDetailDto MapToDto(Tag tag) =>
        new TagAdminDetailDto
        {
            Id = tag.Id,
            Translations = tag.Translations.Select(t => new TagTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Slug = t.Slug
            }).ToList()
        };
}