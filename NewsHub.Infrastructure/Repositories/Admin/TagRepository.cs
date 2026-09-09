using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.Tags;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Admin;

public class TagRepository : ITagRepository
{
    private readonly NewsHubDbContext _context;

    public TagRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tags
            .Include(t => t.Translations)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<(List<Tag> Items, int TotalCount)> GetAllAsync(TagFilter filter)
    {
        var query = _context.Tags
            .Include(t => t.Translations)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var term = filter.Name.Trim();
            query = query.Where(t => t.Translations.Any(tr => EF.Functions.Like(tr.Name, $"%{term}%")));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task UpdateAsync(Tag tag)
    {
        _context.Tags.Update(tag);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Tag tag)
    {
        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
    }
}