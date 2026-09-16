using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Web.Models.Article;
using NewsHub.Web.Models.Shared;

namespace NewsHub.Web.Controllers;

public class CategoryController : Controller
{
    private const int PageSize = 12;

    private readonly ICategoryService _categoryService;
    private readonly IArticleService _articleService;

    public CategoryController(ICategoryService categoryService, IArticleService articleService)
    {
        _categoryService = categoryService;
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string slug, int page = 1)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var category = await _categoryService.GetBySlugAsync(slug);
        if (category is null)
            return NotFound();

        var filter = new ArticleListFilter { Page = page, PageSize = PageSize, CategoryId = category.Id };
        var result = await _articleService.GetPagedAsync(filter);
        var totalPages = result.PageSize == 0 ? 0 : (int)Math.Ceiling(result.TotalCount / (double)result.PageSize);

        var vm = new ArticleListViewModel
        {
            Result = result,
            Heading = category.Name,
            Pagination = new PaginationViewModel
            {
                CurrentPage = result.Page,
                TotalPages = totalPages,
                Controller = "Category",
                Action = "Index",
                RouteValues = new { slug }
            }
        };

        return View(vm);
    }
}