using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.Categories;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Admin;

public class CategoryRepository : ICategoryRepository
{
    private readonly NewsHubDbContext _context;

    public CategoryRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Translations)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(List<Category> Items, int TotalCount)> GetAllAsync(CategoryFilter filter)
    {
        var query = _context.Categories
            .Include(c => c.Translations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var term = filter.Name.Trim();
            query = query.Where(c => c.Translations.Any(t => EF.Functions.Like(t.Name, $"%{term}%")));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}