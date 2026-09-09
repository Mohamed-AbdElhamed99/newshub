using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class CommentRepository : ICommentRepository
{
    private readonly NewsHubDbContext _context;

    public CommentRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<Dictionary<int, int>> GetApprovedCommentCountsAsync(IEnumerable<int> articleIds)
    {
        var ids = articleIds.ToList();

        return await _context.Comments
            .Where(c => ids.Contains(c.ArticleId) && c.Status == CommentStatus.Approved)
            .GroupBy(c => c.ArticleId)
            .Select(g => new { ArticleId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ArticleId, x => x.Count);
    }

    public async Task AddAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Comment>> GetByArticleAndUserAsync(int articleId, Guid userId)
    {
        return await _context.Comments
            .Where(c => c.ArticleId == articleId && c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }
}
