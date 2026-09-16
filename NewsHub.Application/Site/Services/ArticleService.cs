using System.Globalization;
using NewsHub.Application.Common.Pagination;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _repository;
    private readonly ITagRepository _tagRepository;

    public ArticleService(IArticleRepository repository, ITagRepository tagRepository)
    {
        _repository = repository;
        _tagRepository = tagRepository;
    }

    public async Task<IEnumerable<TrendingArticleDto>> GetTrendingAsync(int count)
    {
        var articles = await _repository.GetTrendingAsync(count);
        return articles.Select(MapToTrendingDto).ToList();
    }

    public async Task<IEnumerable<LatestArticleDto>> GetLatestAsync(int count)
    {
        var results = await _repository.GetLatestAsync(count);
        return results.Select(MapToLatestDto).ToList();
    }

    public async Task<TopStoryDto?> GetTopStoryAsync()
    {
        var article = await _repository.GetTopStoryAsync();
        return article == null ? null : MapToTopStoryDto(article);
    }

    public async Task<IEnumerable<LatestArticleDto>> GetMostViewedAsync(int count)
    {
        var results = await _repository.GetMostViewedAsync(count);
        return results.Select(MapToLatestDto).ToList();
    }

    public async Task<IEnumerable<PopularArticleDto>> GetPopularAsync(int count)
    {
        var results = await _repository.GetHighestRatedAsync(count);
        return results.Select(MapToPopularDto).ToList();
    }

    public async Task<CategoryArticlesDto> GetByCategoryAsync(int categoryId, string categoryName, int count)
    {
        var results = await _repository.GetByCategoryAsync(categoryId, count);

        return new CategoryArticlesDto
        {
            CategoryId = categoryId,
            CategoryName = categoryName,
            Articles = results.Select(MapToLatestDto).ToList()
        };
    }

    public async Task<ArticleDetailDto?> GetArticleDetailBySlugAsync(string slug)
    {
        var article = await _repository.GetBySlugAsync(slug);
        if (article == null) return null;

        var commentCount = await _repository.GetApprovedCommentCountAsync(article.Id);

        var tagIds = article.Tags.Select(t => t.TagId).ToList();
        var tags = tagIds.Count > 0
            ? await _tagRepository.GetByIdsAsync(tagIds)
            : Enumerable.Empty<Tag>();

        var tagNames = tags.Select(ResolveTagName).ToList();

        return MapToDetailDto(article, commentCount, tagNames);
    }

    public async Task<PagedResult<LatestArticleDto>> GetPagedAsync(ArticleListFilter filter)
    {
        var (results, totalCount) = await _repository.GetPagedAsync(filter);

        var items = results.Select(MapToLatestDto).ToList();

        return new PagedResult<LatestArticleDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public Task RegisterViewAsync(int articleId) => _repository.IncrementViewCountAsync(articleId);

    private static ArticleTranslation ResolveTranslation(Article article)
    {
        return article.Translations
            .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
            ?? article.Translations.FirstOrDefault(t => t.LanguageCode == "en")
            ?? article.Translations.First();
    }

    private static TrendingArticleDto MapToTrendingDto(Article article)
    {
        var translation = ResolveTranslation(article);

        return new TrendingArticleDto
        {
            Id = article.Id,
            Title = translation.Title,
            Excerpt = translation.Excerpt,
            ImageUrl = article.ImageUrl,
            Slug = translation.Slug,
            LanguageCode = translation.LanguageCode
        };
    }

    private static LatestArticleDto MapToLatestDto(ArticleWithCommentCount result)
    {
        var article = result.Article;
        var translation = ResolveTranslation(article);

        return new LatestArticleDto
        {
            Id = article.Id,
            Title = translation.Title,
            Excerpt = translation.Excerpt,
            ImageUrl = article.ImageUrl,
            PublishedAt = article.PublishedAt,
            ViewCount = article.ViewCount,
            CommentCount = result.CommentCount,
            Slug = translation.Slug,
            LanguageCode = translation.LanguageCode
        };
    }

    private static TopStoryDto MapToTopStoryDto(Article article)
    {
        var translation = ResolveTranslation(article);

        return new TopStoryDto
        {
            Id = article.Id,
            Title = translation.Title,
            Excerpt = translation.Excerpt,
            ImageUrl = article.ImageUrl,
            ViewCount = article.ViewCount,
            Slug = translation.Slug,
            LanguageCode = translation.LanguageCode
        };
    }

    private static PopularArticleDto MapToPopularDto(ArticleWithRating result)
    {
        var article = result.Article;
        var translation = ResolveTranslation(article);

        return new PopularArticleDto
        {
            Id = article.Id,
            Title = translation.Title,
            ImageUrl = article.ImageUrl,
            AverageRating = result.AverageRating,
            Slug = translation.Slug,
            LanguageCode = translation.LanguageCode
        };
    }

    private static ArticleDetailDto MapToDetailDto(Article article, int commentCount, List<string> tagNames)
    {
        var translation = ResolveTranslation(article);

        return new ArticleDetailDto
        {
            Id = article.Id,
            Title = translation.Title,
            Content = translation.Content,
            Excerpt = translation.Excerpt,
            MetaTitle = translation.MetaTitle,
            MetaDescription = translation.MetaDescription,
            ImageUrl = article.ImageUrl,
            CategoryId = article.CategoryId,
            PublishedAt = article.PublishedAt,
            ViewCount = article.ViewCount,
            CommentCount = commentCount,
            Slug = translation.Slug,
            LanguageCode = translation.LanguageCode,
            TagNames = tagNames
        };
    }
    
    private static string ResolveTagName(Tag tag)
    {
        var translation = tag.Translations
                              .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                          ?? tag.Translations.FirstOrDefault(t => t.LanguageCode == "en")
                          ?? tag.Translations.First();

        return translation.Name;
    }
}