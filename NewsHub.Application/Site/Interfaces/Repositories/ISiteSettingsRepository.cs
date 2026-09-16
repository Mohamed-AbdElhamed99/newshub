using NewsHub.Application.Site.DTOs.Layout;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface ISiteSettingsRepository
{
    Task<ContactInfoDto> GetContactInfoAsync();
    Task<IEnumerable<SocialLinkDto>> GetSocialLinksAsync();
}