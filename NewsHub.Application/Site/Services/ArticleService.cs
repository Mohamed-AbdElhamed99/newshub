using System.Globalization;
using NewsHub.Application.Site.DTOs;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class ArticleService : IArticleService
{
    private readonly IArticleRepository _repository;
    private readonly ICommentRepository _commentRepository;

    public ArticleService(IArticleRepository repository, ICommentRepository commentRepository)
    {
        _repository = repository;
        _commentRepository = commentRepository;
    }

    public async Task<IEnumerable<TrendingArticleDto>> GetTrendingAsync(int count)
    {
        var articles = await _repository.GetTrendingAsync(count);
        return articles.Select(MapToTrendingDto).ToList();
    }

    public async Task<IEnumerable<LatestArticleDto>> GetLatestAsync(int count)
    {
        var articles = (await _repository.GetLatestAsync(count)).ToList();
        var commentCounts = await _commentRepository.GetApprovedCommentCountsAsync(articles.Select(a => a.Id));

        return articles.Select(a => MapToLatestDto(a, commentCounts)).ToList();
    }

    public async Task<TopStoryDto?> GetTopStoryAsync()
    {
        var article = await _repository.GetTopStoryAsync();
        return article == null ? null : MapToTopStoryDto(article);
    }

    public async Task<IEnumerable<LatestArticleDto>> GetMostViewedAsync(int count)
    {
        var articles = (await _repository.GetMostViewedAsync(count)).ToList();
        var commentCounts = await _commentRepository.GetApprovedCommentCountsAsync(articles.Select(a => a.Id));

        return articles.Select(a => MapToLatestDto(a, commentCounts)).ToList();
    }

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

    private static LatestArticleDto MapToLatestDto(Article article, Dictionary<int, int> commentCounts)
    {
        var translation = ResolveTranslation(article);

        return new LatestArticleDto
        {
            Id = article.Id,
            Title = translation.Title,
            Excerpt = translation.Excerpt,
            ImageUrl = article.ImageUrl,
            PublishedAt = article.PublishedAt,
            ViewCount = article.ViewCount,
            CommentCount = commentCounts.GetValueOrDefault(article.Id, 0),
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
}