namespace NewsHub.Infrastructure.Repositories.Admin;

using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

public class SiteSettingRepository : ISiteSettingRepository
{
    private readonly NewsHubDbContext _context;

    public SiteSettingRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<SiteSetting?> GetAsync()
    {
        return await _context.SiteSettings.FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(SiteSetting settings)
    {
        if (settings.Id == 0)
        {
            _context.SiteSettings.Add(settings);
        }
        else
        {
            _context.SiteSettings.Update(settings);
        }

        await _context.SaveChangesAsync();
    }
}