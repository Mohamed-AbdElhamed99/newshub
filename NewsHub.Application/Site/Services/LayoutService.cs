using System.Globalization;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.DTOs.Layout;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class LayoutService : ILayoutService
{
    private readonly IArticleService _articleService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISiteSettingsRepository _siteSettingsRepository;

    public LayoutService(
        IArticleService articleService,
        ICategoryRepository categoryRepository,
        ISiteSettingsRepository siteSettingsRepository)
    {
        _articleService = articleService;
        _categoryRepository = categoryRepository;
        _siteSettingsRepository = siteSettingsRepository;
    }

    public async Task<IEnumerable<LatestArticleDto>> GetRecentPostsAsync(int count)
    {
        return await _articleService.GetLatestAsync(count);
    }

    public async Task<IEnumerable<CategoryDto>> GetFooterCategoriesAsync(int count)
    {
        var categories = await _categoryRepository.GetTopNAsync(count);
        return categories.Select(MapToCategoryDto).ToList();
    }

    public async Task<IEnumerable<string>> GetGalleryImageUrlsAsync(int count)
    {
        var recentPosts = await _articleService.GetLatestAsync(count);
        return recentPosts.Select(p => p.ImageUrl).ToList();
    }

    public async Task<ContactInfoDto> GetContactInfoAsync()
    {
        return await _siteSettingsRepository.GetContactInfoAsync();
    }

    public async Task<IEnumerable<SocialLinkDto>> GetSocialLinksAsync()
    {
        return await _siteSettingsRepository.GetSocialLinksAsync();
    }

    private static CategoryDto MapToCategoryDto(Category category)
    {
        var translation = category.Translations
            .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
            ?? category.Translations.FirstOrDefault(t => t.LanguageCode == "en")
            ?? category.Translations.First();

        return new CategoryDto
        {
            Id = category.Id,
            Name = translation.Name,
            Slug = translation.Slug
        };
    }
}