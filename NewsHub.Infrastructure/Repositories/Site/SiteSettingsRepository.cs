using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.DTOs.Layout;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class SiteSettingsRepository : ISiteSettingsRepository
{
    private readonly NewsHubDbContext _context;

    public SiteSettingsRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<ContactInfoDto> GetContactInfoAsync()
    {
        var settings = await _context.SiteSettings.FirstOrDefaultAsync();

        if (settings is null)
        {
            return new ContactInfoDto();
        }

        return new ContactInfoDto
        {
            Email = settings.ContactEmail,
            Phone = settings.ContactPhone,
            Address = settings.ContactAddress
        };
    }

    public async Task<IEnumerable<SocialLinkDto>> GetSocialLinksAsync()
    {
        return await _context.SocialLinks
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .Select(s => new SocialLinkDto { Platform = s.Platform, Url = s.Url })
            .ToListAsync();
    }
}
