using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Admin.DTOs.Comments;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Domain.Enums;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]")]
public class CommentsController : Controller
{
    private readonly ICommentAdminService _service;

    public CommentsController(ICommentAdminService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(CommentStatus? status, int page = 1)
    {
        var filter = new CommentFilter
        {
            Status = status,
            Page = page,
            PageSize = 20
        };

        var result = await _service.GetCommentsAsync(filter);

        ViewData["StatusFilter"] = status;

        return View(result);
    }

    [HttpPost("Approve/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var comment = await _service.ApproveCommentAsync(id);

        TempData[comment != null ? "Success" : "Error"] =
            comment != null ? "Comment approved." : "Comment not found.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Reject/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var comment = await _service.RejectCommentAsync(id);

        TempData[comment != null ? "Success" : "Error"] =
            comment != null ? "Comment rejected." : "Comment not found.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteCommentAsync(id);

        TempData[deleted ? "Success" : "Error"] =
            deleted ? "Comment deleted." : "Comment not found.";

        return RedirectToAction(nameof(Index));
    }
}