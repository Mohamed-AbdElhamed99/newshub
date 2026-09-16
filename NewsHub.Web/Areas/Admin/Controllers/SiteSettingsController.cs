using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Admin.DTOs.SiteSettings;
using NewsHub.Application.Admin.DTOs.SocialLinks;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Web.Areas.Admin.ViewModels.SiteSettings;
using NewsHub.Web.Areas.Admin.ViewModels.SocialLinks;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/[controller]")]
public class SiteSettingsController : Controller
{
    private readonly ISiteSettingAdminService _settingsService;
    private readonly ISocialLinkAdminService _socialLinkService;

    public SiteSettingsController(
        ISiteSettingAdminService settingsService,
        ISocialLinkAdminService socialLinkService)
    {
        _settingsService = settingsService;
        _socialLinkService = socialLinkService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var settings = await _settingsService.GetAsync();
        var links = await _socialLinkService.GetAllAsync();

        ViewData["SocialLinks"] = links;

        var vm = new SiteSettingFormViewModel
        {
            ContactEmail = settings.ContactEmail,
            ContactPhone = settings.ContactPhone,
            ContactAddress = settings.ContactAddress
        };

        return View(vm);
    }

    [HttpPost("UpdateSettings")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSettings(SiteSettingFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewData["SocialLinks"] = await _socialLinkService.GetAllAsync();
            return View(nameof(Index), vm);
        }

        await _settingsService.UpdateAsync(new UpdateSiteSettingDto
        {
            ContactEmail = vm.ContactEmail,
            ContactPhone = vm.ContactPhone,
            ContactAddress = vm.ContactAddress
        });

        TempData["Success"] = "Site settings updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("SocialLinks/Create")]
    public IActionResult CreateSocialLink()
    {
        return View("SocialLinks/CreateSocialLink" , new SocialLinkFormViewModel());
    }

    [HttpPost("SocialLinks/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSocialLink(SocialLinkFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _socialLinkService.CreateAsync(new SocialLinkUpsertDto
        {
            Platform = vm.Platform,
            Url = vm.Url,
            IconClass = vm.IconClass,
            DisplayOrder = vm.DisplayOrder,
            IsActive = vm.IsActive
        });

        TempData["Success"] = "Social link created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("SocialLinks/Edit/{id:int}")]
    public async Task<IActionResult> EditSocialLink(int id)
    {
        var link = await _socialLinkService.GetByIdAsync(id);
        if (link == null) return NotFound();

        var vm = new SocialLinkFormViewModel
        {
            Id = link.Id,
            Platform = link.Platform,
            Url = link.Url,
            IconClass = link.IconClass,
            DisplayOrder = link.DisplayOrder,
            IsActive = link.IsActive
        };

        return View("SocialLinks/EditSocialLink" , vm);
    }

    [HttpPost("SocialLinks/Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSocialLink(int id, SocialLinkFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var updated = await _socialLinkService.UpdateAsync(new SocialLinkUpsertDto
        {
            Id = vm.Id,
            Platform = vm.Platform,
            Url = vm.Url,
            IconClass = vm.IconClass,
            DisplayOrder = vm.DisplayOrder,
            IsActive = vm.IsActive
        });

        TempData[updated != null ? "Success" : "Error"] =
            updated != null ? "Social link updated." : "Social link not found.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost("SocialLinks/Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSocialLink(int id)
    {
        var deleted = await _socialLinkService.DeleteAsync(id);

        TempData[deleted ? "Success" : "Error"] =
            deleted ? "Social link deleted." : "Social link not found.";

        return RedirectToAction(nameof(Index));
    }
}