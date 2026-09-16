using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class TagRepository : ITagRepository
{
    private readonly NewsHubDbContext _context;

    public TagRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    // "Trending" tags = the ones attached to the most articles.
    public async Task<IEnumerable<Tag>> GetTrendingTagsAsync(int count)
    {
        return await _context.Tags
            .Include(t => t.Translations)
            .OrderByDescending(t => _context.ArticleTags.Count(at => at.TagId == t.Id))
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<int> tagIds)
    {
        var ids = tagIds.ToList();

        return await _context.Tags
            .Include(t => t.Translations)
            .Where(t => ids.Contains(t.Id))
            .ToListAsync();
    }
}
