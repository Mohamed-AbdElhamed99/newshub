using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.Interfaces.Services;

namespace NewsHub.Web.ViewComponents;

public class CategoryNavViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;

    public CategoryNavViewComponent(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        return View(categories);
    }
}