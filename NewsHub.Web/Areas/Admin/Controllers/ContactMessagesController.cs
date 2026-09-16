using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Admin.DTOs.ContactMessages;
using NewsHub.Application.Admin.Interfaces.Services;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]")]
public class ContactMessagesController : Controller
{
    private readonly IContactMessageAdminService _service;

    public ContactMessagesController(IContactMessageAdminService service)
    {
        _service = service;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(bool? isRead, int page = 1)
    {
        var filter = new ContactMessageFilter
        {
            IsRead = isRead,
            Page = page,
            PageSize = 20
        };

        var result = await _service.GetMessagesAsync(filter);

        ViewData["IsReadFilter"] = isRead;

        return View(result);
    }

    [HttpPost("MarkAsRead/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var message = await _service.MarkAsReadAsync(id);

        TempData[message != null ? "Success" : "Error"] =
            message != null ? "Message marked as read." : "Message not found.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteMessageAsync(id);

        TempData[deleted ? "Success" : "Error"] =
            deleted ? "Message deleted." : "Message not found.";

        return RedirectToAction(nameof(Index));
    }
}