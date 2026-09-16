namespace NewsHub.Infrastructure.Repositories.Admin;

using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

public class SocialLinkRepository : ISocialLinkRepository
{
    private readonly NewsHubDbContext _context;

    public SocialLinkRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<List<SocialLink>> GetAllAsync()
    {
        return await _context.SocialLinks
            .OrderBy(l => l.DisplayOrder)
            .ToListAsync();
    }

    public async Task<SocialLink?> GetByIdAsync(int id)
    {
        return await _context.SocialLinks.FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task AddAsync(SocialLink link)
    {
        _context.SocialLinks.Add(link);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(SocialLink link)
    {
        _context.SocialLinks.Update(link);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(SocialLink link)
    {
        _context.SocialLinks.Remove(link);
        await _context.SaveChangesAsync();
    }
}