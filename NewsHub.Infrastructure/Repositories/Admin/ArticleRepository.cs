using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.Articles;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Admin;

public class ArticleRepository : IArticleRepository
{
    private readonly NewsHubDbContext _dbContext;
    public ArticleRepository(NewsHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Article article)
    {
        await _dbContext.Articles.AddAsync(article);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Article?> GetByIdAsync(int id)
    {
        var result = await _dbContext.Articles.Include(a => a.Translations).FirstOrDefaultAsync(a => a.Id == id);
        return result;
    }

    public async Task<(List<Article> Items, int TotalCount)> GetAllAsync(ArticleFilter filter)
    {
        var query = _dbContext.Articles.Include(a => a.Translations).AsQueryable();

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == filter.CategoryId.Value);
        }


        if (filter.Status.HasValue)
        {
            query = query.Where(a => a.Status == filter.Status.Value);
        }
            

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(a => a.Translations.Any(t => EF.Functions.Like(t.Title, $"%{term}%")));
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }
    
    public async Task UpdateAsync(Article article)
    {
        _dbContext.Articles.Update(article);
        await _dbContext.SaveChangesAsync();
    }
 
    public async Task DeleteAsync(Article article)
    {
        _dbContext.Articles.Remove(article);
        await _dbContext.SaveChangesAsync();
    }
}