using NewsHub.Application.Common.Random;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class HomePageService : IHomePageService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IArticleService _articleService;
    private readonly IRandomProvider _randomProvider;

    public HomePageService(
        ICategoryRepository categoryRepository,
        IArticleService articleService,
        IRandomProvider randomProvider)
    {
        _categoryRepository = categoryRepository;
        _articleService = articleService;
        _randomProvider = randomProvider;
    }

    public async Task<IEnumerable<CategoryArticlesDto>> GetWhatIsNewAsync(int articlesPerCategory)
    {
        var categories = await _categoryRepository.GetAllAsync();

        var result = new List<CategoryArticlesDto>();

        foreach (var category in categories)
        {
            var categoryName = ResolveCategoryName(category);
            var categoryArticles = await _articleService.GetByCategoryAsync(category.Id, categoryName, articlesPerCategory);
            result.Add(categoryArticles);
        }

        return result;
    }

    public async Task<CategoryArticlesDto> GetLifeStyleSectionAsync(int articleCount)
    {
        var categories = (await _categoryRepository.GetAllAsync()).ToList();

        var targetCategory = FindLifeStyleCategory(categories) ?? PickRandomCategory(categories);

        if (targetCategory == null)
        {
            // No categories exist at all — nothing to show
            return new CategoryArticlesDto { CategoryId = 0, CategoryName = string.Empty, Articles = new List<LatestArticleDto>() };
        }

        var categoryName = ResolveCategoryName(targetCategory);
        return await _articleService.GetByCategoryAsync(targetCategory.Id, categoryName, articleCount);
    }

    private static Category? FindLifeStyleCategory(List<Category> categories)
    {
        return categories.FirstOrDefault(c =>
            c.Translations.Any(t => t.Slug == WellKnownCategorySlugs.LifeStyle));
    }

    private Category? PickRandomCategory(List<Category> categories)
    {
        if (categories.Count == 0) return null;

        var index = _randomProvider.Next(categories.Count);
        return categories[index];
    }

    private static string ResolveCategoryName(Category category)
    {
        var translation = category.Translations
            .FirstOrDefault(t => t.LanguageCode == System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
            ?? category.Translations.FirstOrDefault(t => t.LanguageCode == "en")
            ?? category.Translations.First();

        return translation.Name;
    }
}