using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Web.Models.Home;

namespace NewsHub.Web.Controllers;

public class HomeController : Controller
{
    private readonly IArticleService _articleService;
    private readonly IHomePageService _homePageService;
    private readonly ITagService _tagService;
    private readonly ILayoutService _layoutService;

    public HomeController(
        IArticleService articleService,
        IHomePageService homePageService,
        ITagService tagService,
        ILayoutService layoutService)
    {
        _articleService = articleService;
        _homePageService = homePageService;
        _tagService = tagService;
        _layoutService = layoutService;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeIndexViewModel
        {
            TopStory = await _articleService.GetTopStoryAsync(),
            MainSectionSidebar = await _articleService.GetLatestAsync(7),
            LatestNewsCarousel = await _articleService.GetLatestAsync(5),
            WhatsNew = await _homePageService.GetWhatIsNewAsync(6),
            MostViewed = await _articleService.GetMostViewedAsync(5),
            LifeStyle = await _homePageService.GetLifeStyleSectionAsync(2),
            PopularNews = await _articleService.GetPopularAsync(4),
            TrendingTags = await _tagService.GetTrendingTagsAsync(8),
            SocialLinks = await _layoutService.GetSocialLinksAsync()
        };

        return View(viewModel);
    }
}