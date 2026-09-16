using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Admin.DTOs.Tags;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Web.Areas.Admin.ViewModels.Tags;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Tags")]
public class TagsController : Controller
{
    private readonly ITagAdminService _tagService;

    public TagsController(ITagAdminService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? name, int page = 1, int pageSize = 10)
    {
        var filter = new TagFilter { Name = name, Page = page, PageSize = pageSize };
        var result = await _tagService.GetTagsAsync(filter);
        ViewData["NameFilter"] = name;
        return View(result);
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new TagFormViewModel());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TagFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var dto = new CreateTagDto { Translations = BuildTranslations(model) };
        await _tagService.CreateTagAsync(dto);

        TempData["Success"] = "Tag created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var tag = await _tagService.GetTagByIdAsync(id);
        if (tag == null) return NotFound();

        var en = tag.Translations.FirstOrDefault(t => t.LanguageCode == "en");
        var ar = tag.Translations.FirstOrDefault(t => t.LanguageCode == "ar");

        var model = new TagFormViewModel
        {
            Id = tag.Id,
            English = new TagTranslationViewModel { LanguageCode = "en", Name = en?.Name ?? "", Slug = en?.Slug ?? "" },
            Arabic = new TagTranslationViewModel { LanguageCode = "ar", Name = ar?.Name ?? "", Slug = ar?.Slug ?? "" }
        };

        return View(model);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TagFormViewModel model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var dto = new UpdateTagDto { Id = model.Id, Translations = BuildTranslations(model) };
        var updated = await _tagService.UpdateTagAsync(dto);
        if (updated == null) return NotFound();

        TempData["Success"] = "Tag updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _tagService.DeleteTagAsync(id);
        if (!deleted) return NotFound();

        TempData["Success"] = "Tag deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static List<TagTranslationDto> BuildTranslations(TagFormViewModel model) => new()
    {
        new() { LanguageCode = "en", Name = model.English.Name, Slug = model.English.Slug },
        new() { LanguageCode = "ar", Name = model.Arabic.Name, Slug = model.Arabic.Slug }
    };
}