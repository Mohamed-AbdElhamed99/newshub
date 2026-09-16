using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.DTOs.Layout;

namespace NewsHub.Web.Models.Shared;

public class FooterViewModel
{
    public ContactInfoDto? ContactInfo { get; set; }
    public IEnumerable<SocialLinkDto> SocialLinks { get; set; } = Enumerable.Empty<SocialLinkDto>();
    public IEnumerable<LatestArticleDto> RecentPosts { get; set; } = Enumerable.Empty<LatestArticleDto>();
    public IEnumerable<CategoryDto> Categories { get; set; } = Enumerable.Empty<CategoryDto>();
    public IEnumerable<string> GalleryImageUrls { get; set; } = Enumerable.Empty<string>();
}