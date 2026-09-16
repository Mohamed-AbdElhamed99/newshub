using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class ArticleRepository : IArticleRepository
{
    private readonly NewsHubDbContext _context;

    public ArticleRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    // Site-facing queries only ever surface published articles.
    private IQueryable<Article> PublishedQuery() =>
        _context.Articles
            .Include(a => a.Translations)
            .Where(a => a.Status == ArticleStatus.Published);

    public async Task<Article?> GetByIdAsync(int id)
    {
        return await PublishedQuery().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Article?> GetBySlugAsync(string slug)
    {
        return await PublishedQuery()
            .FirstOrDefaultAsync(a => a.Translations.Any(t => t.Slug == slug));
    }

    public async Task<IEnumerable<Article>> GetTrendingAsync(int count)
    {
        return await PublishedQuery()
            .Where(a => a.IsTrending)
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticleWithCommentCount>> GetLatestAsync(int count)
    {
        return await PublishedQuery()
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .Select(a => new ArticleWithCommentCount
            {
                Article = a,
                CommentCount = a.Comments.Count(c => c.Status == CommentStatus.Approved)
            })
            .ToListAsync();
    }

    public async Task<Article?> GetTopStoryAsync()
    {
        return await PublishedQuery()
            .OrderByDescending(a => a.IsTrending)
            .ThenByDescending(a => a.ViewCount)
            .ThenByDescending(a => a.PublishedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ArticleWithCommentCount>> GetMostViewedAsync(int count)
    {
        return await PublishedQuery()
            .OrderByDescending(a => a.ViewCount)
            .Take(count)
            .Select(a => new ArticleWithCommentCount
            {
                Article = a,
                CommentCount = a.Comments.Count(c => c.Status == CommentStatus.Approved)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticleWithCommentCount>> GetByCategoryAsync(int categoryId, int count)
    {
        return await PublishedQuery()
            .Where(a => a.CategoryId == categoryId)
            .OrderByDescending(a => a.PublishedAt)
            .Take(count)
            .Select(a => new ArticleWithCommentCount
            {
                Article = a,
                CommentCount = a.Comments.Count(c => c.Status == CommentStatus.Approved)
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ArticleWithRating>> GetHighestRatedAsync(int count)
    {
        return await PublishedQuery()
            .Select(a => new ArticleWithRating
            {
                Article = a,
                AverageRating = a.Ratings.Any() ? a.Ratings.Average(r => r.Rating) : 0
            })
            .OrderByDescending(x => x.AverageRating)
            .Take(count)
            .ToListAsync();
    }

    public async Task<(IEnumerable<ArticleWithCommentCount> Items, int TotalCount)> GetPagedAsync(ArticleListFilter filter)
    {
        var query = PublishedQuery();

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == filter.CategoryId.Value);
        }

        if (filter.TagId.HasValue)
        {
            query = query.Where(a => a.Tags.Any(t => t.TagId == filter.TagId.Value));
        }

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(a => a.Translations.Any(t => EF.Functions.Like(t.Title, $"%{term}%")));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.PublishedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(a => new ArticleWithCommentCount
            {
                Article = a,
                CommentCount = a.Comments.Count(c => c.Status == CommentStatus.Approved)
            })
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<int> GetApprovedCommentCountAsync(int articleId)
    {
        return await _context.Comments
            .CountAsync(c => c.ArticleId == articleId && c.Status == CommentStatus.Approved);
    }

    public async Task IncrementViewCountAsync(int articleId)
    {
        var article = await _context.Articles.FindAsync(articleId);
        if (article is null)
        {
            return;
        }

        article.ViewCount++;
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Article article)
    {
        _context.Articles.Update(article);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Articles.AnyAsync(a => a.Id == id);
    }
}
