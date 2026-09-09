using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.Comments;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Admin;

public class CommentRepository : ICommentRepository
{
    private readonly NewsHubDbContext _context;

    public CommentRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> GetByIdAsync(int id)
    {
        return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<Comment> Items, int TotalCount)> GetAllAsync(CommentFilter filter)
    {
        var query = _context.Comments.AsQueryable();

        if (filter.ArticleId.HasValue)
        {
            query = query.Where(c => c.ArticleId == filter.ArticleId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(c => c.Status == filter.Status.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task UpdateAsync(Comment comment)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Comment comment)
    {
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
    }
}