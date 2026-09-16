using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class ArticleRatingRepository : IArticleRatingRepository
{
    private readonly NewsHubDbContext _context;

    public ArticleRatingRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ArticleRating article)
    {
        await _context.ArticleRatings.AddAsync(article);
        await _context.SaveChangesAsync();
    }

    public async Task<ArticleRating?> GetByArticleAndUserAsync(int articleId, Guid userId)
    {
        return await _context.ArticleRatings
            .FirstOrDefaultAsync(r => r.ArticleId == articleId && r.UserId == userId);
    }

    public async Task UpdateAsync(ArticleRating article)
    {
        _context.ArticleRatings.Update(article);
        await _context.SaveChangesAsync();
    }
}
