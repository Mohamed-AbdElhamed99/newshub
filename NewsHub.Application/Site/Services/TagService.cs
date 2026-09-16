using NewsHub.Application.Site.Common;
using NewsHub.Application.Site.DTOs.Tags;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _repository;

    public TagService(ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TrendingTagDto>> GetTrendingTagsAsync(int count)
    {
        var tags = await _repository.GetTrendingTagsAsync(count);
        return tags.Select(MapToDto).ToList();
    }

    private static TrendingTagDto MapToDto(Tag tag)
    {
        var name = TranslationResolver.ResolveName(tag.Translations, out var languageCode);
        var translation = tag.Translations
            .First(t => t.LanguageCode == languageCode); // same translation just resolved; re-fetch slug from it

        return new TrendingTagDto
        {
            Id = tag.Id,
            Name = name,
            Slug = translation.Slug,
            LanguageCode = languageCode
        };
    }
}