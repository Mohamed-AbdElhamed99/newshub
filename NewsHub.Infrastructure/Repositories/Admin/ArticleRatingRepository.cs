using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.ArticleRatings;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Admin;

public class ArticleRatingRepository : IArticleRatingRepository
{
    private readonly NewsHubDbContext _context;

    public ArticleRatingRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<ArticleRating?> GetByIdAsync(int id)
    {
        return await _context.ArticleRatings.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<(List<ArticleRating> Items, int TotalCount)> GetAllAsync(ArticleRatingFilter filter)
    {
        var query = _context.ArticleRatings.AsQueryable();

        if (filter.ArticleId.HasValue)
        {
            query = query.Where(r => r.ArticleId == filter.ArticleId.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(double Average, int TotalRatings)> GetSummaryByArticleIdAsync(int articleId)
    {
        var query = _context.ArticleRatings.Where(r => r.ArticleId == articleId);

        var totalRatings = await query.CountAsync();
        if (totalRatings == 0)
        {
            return (0d, 0);
        }

        var average = await query.AverageAsync(r => r.Rating);
        return (average, totalRatings);
    }

    public async Task DeleteAsync(ArticleRating rating)
    {
        _context.ArticleRatings.Remove(rating);
        await _context.SaveChangesAsync();
    }
}