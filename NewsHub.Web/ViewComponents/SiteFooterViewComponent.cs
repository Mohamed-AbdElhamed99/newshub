using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Web.Models.Shared;

namespace NewsHub.Web.ViewComponents;

public class SiteFooterViewComponent : ViewComponent
{
    private readonly ILayoutService _layoutService;

    public SiteFooterViewComponent(ILayoutService layoutService)
    {
        _layoutService = layoutService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var viewModel = new FooterViewModel
        {
            ContactInfo = await _layoutService.GetContactInfoAsync(),
            SocialLinks = await _layoutService.GetSocialLinksAsync(),
            RecentPosts = await _layoutService.GetRecentPostsAsync(2),
            Categories = await _layoutService.GetFooterCategoriesAsync(6),
            GalleryImageUrls = await _layoutService.GetGalleryImageUrlsAsync(6)
        };

        return View(viewModel);
    }
}