using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.DTOs.Layout;
using NewsHub.Application.Site.DTOs.Tags;

namespace NewsHub.Web.Models.Home;

public class HomeIndexViewModel
{
    public TopStoryDto? TopStory { get; set; }

    // Right-hand column next to the top story
    public IEnumerable<LatestArticleDto> MainSectionSidebar { get; set; } = Enumerable.Empty<LatestArticleDto>();

    // "Latest News" carousel
    public IEnumerable<LatestArticleDto> LatestNewsCarousel { get; set; } = Enumerable.Empty<LatestArticleDto>();

    // "What's New" tabbed section — one tab per category
    public IEnumerable<CategoryArticlesDto> WhatsNew { get; set; } = Enumerable.Empty<CategoryArticlesDto>();

    // "Most Views News" carousel
    public IEnumerable<LatestArticleDto> MostViewed { get; set; } = Enumerable.Empty<LatestArticleDto>();

    // "Life Style" section
    public CategoryArticlesDto? LifeStyle { get; set; }

    // Sidebar
    public IEnumerable<PopularArticleDto> PopularNews { get; set; } = Enumerable.Empty<PopularArticleDto>();
    public IEnumerable<TrendingTagDto> TrendingTags { get; set; } = Enumerable.Empty<TrendingTagDto>();
    public IEnumerable<SocialLinkDto> SocialLinks { get; set; } = Enumerable.Empty<SocialLinkDto>();
}