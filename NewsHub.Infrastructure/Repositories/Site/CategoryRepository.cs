using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class CategoryRepository : ICategoryRepository
{
    private readonly NewsHubDbContext _context;

    public CategoryRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    // "Top" categories = the ones with the most published articles, since
    // Category itself carries no ranking field of its own.
    public async Task<IEnumerable<Category>> GetTopNAsync(int count)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .OrderByDescending(c => _context.Articles.Count(a =>
                a.CategoryId == c.Id && a.Status == ArticleStatus.Published))
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .OrderBy(c => c.Id)
            .ToListAsync();
    }
}
