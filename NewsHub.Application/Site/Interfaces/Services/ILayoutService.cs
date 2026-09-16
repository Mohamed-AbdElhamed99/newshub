using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.DTOs.Layout;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface ILayoutService
{
    Task<IEnumerable<LatestArticleDto>> GetRecentPostsAsync(int count);
    Task<IEnumerable<CategoryDto>> GetFooterCategoriesAsync(int count);
    Task<IEnumerable<string>> GetGalleryImageUrlsAsync(int count);
    Task<ContactInfoDto> GetContactInfoAsync();
    Task<IEnumerable<SocialLinkDto>> GetSocialLinksAsync();
}