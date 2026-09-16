using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Admin.DTOs.ArticleRatings;
using NewsHub.Application.Admin.Interfaces.Services;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]")]
public class ArticleRatingsController : Controller
{
    private readonly IArticleRatingAdminService _service;

    public ArticleRatingsController(IArticleRatingAdminService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int? articleId, int page = 1)
    {
        var filter = new ArticleRatingFilter
        {
            ArticleId = articleId,
            Page = page,
            PageSize = 20
        };

        var result = await _service.GetRatingsAsync(filter);

        ViewData["ArticleIdFilter"] = articleId;

        return View(result);
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteRatingAsync(id);

        TempData[deleted ? "Success" : "Error"] =
            deleted ? "Rating deleted." : "Rating not found.";

        return RedirectToAction(nameof(Index));
    }
}