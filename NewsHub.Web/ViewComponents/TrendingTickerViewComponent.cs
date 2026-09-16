using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.Interfaces.Services;

namespace NewsHub.Web.ViewComponents;

public class TrendingTickerViewComponent : ViewComponent
{
    private readonly IArticleService _articleService;

    public TrendingTickerViewComponent(IArticleService articleService)
    {
        _articleService = articleService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var trending = await _articleService.GetTrendingAsync(1);
        return View(trending.FirstOrDefault());
    }
}