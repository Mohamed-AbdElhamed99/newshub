using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Admin.DTOs.Subscriptions;
using NewsHub.Application.Admin.Interfaces.Services;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]")]
public class SubscriptionsController : Controller
{
    private readonly ISubscriptionAdminService _service;

    public SubscriptionsController(ISubscriptionAdminService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(bool? isActive, int page = 1)
    {
        var filter = new SubscriptionFilter
        {
            IsActive = isActive,
            Page = page,
            PageSize = 20
        };

        var result = await _service.GetSubscriptionsAsync(filter);

        ViewData["IsActiveFilter"] = isActive;

        return View(result);
    }

    [HttpPost("Deactivate/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        var subscription = await _service.DeactivateSubscriptionAsync(id);

        TempData[subscription != null ? "Success" : "Error"] =
            subscription != null ? "Subscription deactivated." : "Subscription not found.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteSubscriptionAsync(id);

        TempData[deleted ? "Success" : "Error"] =
            deleted ? "Subscription deleted." : "Subscription not found.";

        return RedirectToAction(nameof(Index));
    }
}