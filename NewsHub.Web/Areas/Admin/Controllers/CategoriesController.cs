using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NewsHub.Application.Admin.DTOs.Categories;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Web.Areas.Admin.ViewModels.Categories;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/categories")]
public class CategoriesController : Controller
{
    private readonly ICategoryAdminService _categoryService;
    private readonly IWebHostEnvironment _env;
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public CategoriesController(ICategoryAdminService categoryService, IWebHostEnvironment env)
    {
        _categoryService = categoryService;
        _env = env;
    }
    
      [HttpGet("")]
    public async Task<IActionResult> Index(string? name, int page = 1, int pageSize = 10)
    {
        var filter = new CategoryFilter { Name = name, Page = page, PageSize = pageSize };
        var result = await _categoryService.GetCategoriesAsync(filter);
        ViewData["NameFilter"] = name;
        return View(result);
    }

    [HttpGet("Create")]
    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        string? imagePath;
        try
        {
            imagePath = await SaveImageAsync(model.ImageFile);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
            return View(model);
        }

        var dto = new CreateCategoryDto
        {
            ImagePath = imagePath,
            Translations = BuildTranslations(model)
        };

        await _categoryService.CreateCategoryAsync(dto);
        TempData["Success"] = "Category created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        var en = category.Translations.FirstOrDefault(t => t.LanguageCode == "en");
        var ar = category.Translations.FirstOrDefault(t => t.LanguageCode == "ar");

        var model = new CategoryFormViewModel
        {
            Id = category.Id,
            ExistingImagePath = category.ImagePath,
            English = new CategoryTranslationViewModel
            {
                LanguageCode = "en", Name = en?.Name ?? "", Slug = en?.Slug ?? "", Description = en?.Description
            },
            Arabic = new CategoryTranslationViewModel
            {
                LanguageCode = "ar", Name = ar?.Name ?? "", Slug = ar?.Slug ?? "", Description = ar?.Description
            }
        };

        return View(model);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var category = await _categoryService.GetCategoryByIdAsync(id);

        var imagePath = category.ImagePath;
        if (model.ImageFile != null)
        {
            try
            {
                imagePath = await SaveImageAsync(model.ImageFile);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
                return View(model);
            }
        }

        var dto = new UpdateCategoryDto
        {
            Id = model.Id,
            ImagePath = imagePath,
            Translations = BuildTranslations(model)
        };

        var updated = await _categoryService.UpdateCategoryAsync(dto);
        if (updated == null) return NotFound();

        TempData["Success"] = "Category updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteCategoryAsync(id);
        if (!deleted) return NotFound();

        TempData["Success"] = "Category deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static List<CategoryTranslationDto> BuildTranslations(CategoryFormViewModel model) => new()
    {
        new() { LanguageCode = "en", Name = model.English.Name, Slug = model.English.Slug, Description = model.English.Description },
        new() { LanguageCode = "ar", Name = model.Arabic.Name, Slug = model.Arabic.Slug, Description = model.Arabic.Description }
    };

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            throw new InvalidOperationException("Only JPG, PNG or WEBP images are allowed.");

        if (file.Length > 2 * 1024 * 1024)
            throw new InvalidOperationException("Image must be smaller than 2 MB.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/categories/{fileName}";
    }
}