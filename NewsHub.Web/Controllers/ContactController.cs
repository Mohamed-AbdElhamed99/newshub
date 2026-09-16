using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.DTOs.ContactMessages;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Web.Models.Contact;

namespace NewsHub.Web.Controllers;

public class ContactController : Controller
{
    private readonly IContactMessageService _contactMessageService;
    private readonly ILayoutService _layoutService;

    public ContactController(IContactMessageService contactMessageService, ILayoutService layoutService)
    {
        _contactMessageService = contactMessageService;
        _layoutService = layoutService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var vm = new ContactViewModel
        {
            ContactInfo = await _layoutService.GetContactInfoAsync(),
            SocialLinks = await _layoutService.GetSocialLinksAsync()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ContactFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            var vm = new ContactViewModel
            {
                ContactInfo = await _layoutService.GetContactInfoAsync(),
                SocialLinks = await _layoutService.GetSocialLinksAsync(),
                Form = form
            };
            return View("Index", vm);
        }

        await _contactMessageService.SubmitMessageAsync(new ContactMessageDto
        {
            Name = form.Name,
            Email = form.Email,
            Subject = form.Subject,
            Message = form.Message
        });

        TempData["ContactSuccess"] = "Thanks — we've received your message and will get back to you soon.";
        return RedirectToAction(nameof(Index));
    }
}