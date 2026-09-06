using System.Globalization;
using NewsHub.Application.Admin.DTOs.Articles;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Services;

public class ArticleAdminService : IArticleAdminService
{
    private readonly IArticleRepository _repository;

    public ArticleAdminService(IArticleRepository repository)
    {
        this._repository = repository;
    }

    public async Task<ArticleAdminDetailDto> CreateArticleAsync(CreateArticleDto dto)
    {
        var now = DateTime.UtcNow;

        var article = new Article
        {
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl,
            CreatedAt = now,
            UpdatedAt = now,
            Translations = dto.Translations.Select(t => new ArticleTranslation
            {
                LanguageCode = t.LanguageCode,
                Title = t.Title,
                Slug = t.Slug,
                Content = t.Content,
                Excerpt = t.Excerpt,
                MetaTitle = t.MetaTitle,
                MetaDescription = t.MetaDescription
            }).ToList(),
            Tags = dto.TagIds.Select(id => new ArticleTag { TagId = id }).ToList()
        };

        await _repository.AddAsync(article);
        return MapToDto(article);
    }

    public async Task<ArticleAdminDetailDto?> GetArticleByIdAsync(int id)
    {
        var article = await _repository.GetByIdAsync(id);
        return article == null ? null : MapToDto(article);
    }

    public async Task<PagedResult<ArticleAdminDto>> GetArticlesAsync(ArticleFilter filter)
    {
        var (articles, totalCount) = await _repository.GetAllAsync(filter);

        var items = articles.Select(MapToListDto).ToList();

        return new PagedResult<ArticleAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ArticleAdminDetailDto?> UpdateArticleAsync(UpdateArticleDto dto)
    {
        var article = await _repository.GetByIdAsync(dto.Id);
        if (article == null) return null;

        article.CategoryId = dto.CategoryId;
        article.ImageUrl = dto.ImageUrl;
        article.UpdatedAt = DateTime.UtcNow;

        foreach (var t in dto.Translations)
        {
            var existing = article.Translations.FirstOrDefault(e => e.LanguageCode == t.LanguageCode);
            if (existing != null)
            {
                existing.Title = t.Title;
                existing.Slug = t.Slug;
                existing.Content = t.Content;
                existing.Excerpt = t.Excerpt;
                existing.MetaTitle = t.MetaTitle;
                existing.MetaDescription = t.MetaDescription;
            }
            else
            {
                article.Translations.Add(new ArticleTranslation
                {
                    LanguageCode = t.LanguageCode,
                    Title = t.Title,
                    Slug = t.Slug,
                    Content = t.Content,
                    Excerpt = t.Excerpt,
                    MetaTitle = t.MetaTitle,
                    MetaDescription = t.MetaDescription
                });
            }
        }

        var incomingCodes = dto.Translations.Select(t => t.LanguageCode).ToHashSet();
        var translationsToRemove = article.Translations.Where(t => !incomingCodes.Contains(t.LanguageCode)).ToList();
        foreach (var t in translationsToRemove)
            article.Translations.Remove(t);

        var incomingTagIds = dto.TagIds.ToHashSet();
        var existingTagIds = article.Tags.Select(t => t.TagId).ToHashSet();

        var tagsToRemove = article.Tags.Where(t => !incomingTagIds.Contains(t.TagId)).ToList();
        foreach (var t in tagsToRemove)
            article.Tags.Remove(t);

        var tagIdsToAdd = incomingTagIds.Where(id => !existingTagIds.Contains(id));
        foreach (var id in tagIdsToAdd)
            article.Tags.Add(new ArticleTag { TagId = id });

        await _repository.UpdateAsync(article);
        return MapToDto(article);
    }

    public async Task<bool> DeleteArticleAsync(int id)
    {
        var article = await _repository.GetByIdAsync(id);
        if (article == null) return false;

        await _repository.DeleteAsync(article);
        return true;
    }

    public async Task<ArticleAdminDetailDto?> PublishArticleAsync(int id)
    {
        var article = await _repository.GetByIdAsync(id);
        if (article == null) return null;

        article.MarkAsPublished();

        await _repository.UpdateAsync(article);
        return MapToDto(article);
    }

    private static ArticleAdminDto MapToListDto(Article article)
    {
        var translation = article.Translations
            .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
              ?? article.Translations.FirstOrDefault(t => t.LanguageCode == "en")
              ?? article.Translations.First();

        return new ArticleAdminDto
        {
            Id = article.Id,
            Title = translation.Title,
            Slug = translation.Slug,
            CategoryId = article.CategoryId,
            Status = article.Status,
            IsTrending = article.IsTrending,
            ViewCount = article.ViewCount,
            ImageUrl = article.ImageUrl,
            PublishedAt = article.PublishedAt,
            LanguageCode = translation.LanguageCode
        };
    }

    private static ArticleAdminDetailDto MapToDto(Article article) =>
        new ArticleAdminDetailDto
        {
            Id = article.Id,
            AuthorId = article.AuthorId,
            CategoryId = article.CategoryId,
            Status = article.Status,
            IsTrending = article.IsTrending,
            ViewCount = article.ViewCount,
            ImageUrl = article.ImageUrl,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt,
            PublishedAt = article.PublishedAt,
            TagIds = article.Tags.Select(t => t.TagId).ToList(),
            Translations = article.Translations.Select(t => new ArticleTranslationDto
            {
                LanguageCode = t.LanguageCode,
                Title = t.Title,
                Slug = t.Slug,
                Content = t.Content,
                Excerpt = t.Excerpt,
                MetaTitle = t.MetaTitle,
                MetaDescription = t.MetaDescription
            }).ToList()
        };
}