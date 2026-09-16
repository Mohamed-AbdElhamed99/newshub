using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsHub.Application.Admin.DTOs.Articles;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Web.Areas.Admin.ViewModels.Articles;

namespace NewsHub.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Route("Admin/Articles")]
public class ArticlesController : Controller
{
    private readonly IArticleAdminService _articleService;
    private readonly ICategoryAdminService _categoryService;
    private readonly ITagAdminService _tagService;
    private readonly IWebHostEnvironment _env;
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

    public ArticlesController(
        IArticleAdminService articleService,
        ICategoryAdminService categoryService,
        ITagAdminService tagService,
        IWebHostEnvironment env)
    {
        _articleService = articleService;
        _categoryService = categoryService;
        _tagService = tagService;
        _env = env;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? title, int page = 1, int pageSize = 10)
    {
        var filter = new ArticleFilter { Page = page, PageSize = pageSize };
        var result = await _articleService.GetArticlesAsync(filter);
        ViewData["TitleFilter"] = title;
        return View(result);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var model = new ArticleFormViewModel();
        await PopulateOptionsAsync(model);
        return View(model);
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArticleFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        string? imageUrl;
        try
        {
            imageUrl = await SaveImageAsync(model.ImageFile) ?? model.ExistingImageUrl;
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var authorId = GetCurrentUserId();
        if (authorId == null)
        {
            ModelState.AddModelError(string.Empty, "Could not determine the current user.");
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var dto = new CreateArticleDto
        {
            AuthorId = authorId.Value,
            CategoryId = model.CategoryId,
            IsTrending = model.IsTrending,
            ImageUrl = imageUrl ?? string.Empty,
            TagIds = model.SelectedTagIds,
            Translations = BuildTranslations(model)
        };

        await _articleService.CreateArticleAsync(dto);
        TempData["Success"] = "Article created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);
        if (article == null) return NotFound();

        var en = article.Translations.FirstOrDefault(t => t.LanguageCode == "en");
        var ar = article.Translations.FirstOrDefault(t => t.LanguageCode == "ar");

        var model = new ArticleFormViewModel
        {
            Id = article.Id,
            CategoryId = article.CategoryId,
            IsTrending = article.IsTrending,
            Status = article.Status,
            ExistingImageUrl = article.ImageUrl,
            SelectedTagIds = article.TagIds.ToList(),
            English = MapTranslation(en, "en"),
            Arabic = MapTranslation(ar, "ar")
        };

        await PopulateOptionsAsync(model);
        return View(model);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ArticleFormViewModel model)
    {
        if (id != model.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var imageUrl = model.ExistingImageUrl;
        if (model.ImageFile != null)
        {
            try
            {
                imageUrl = await SaveImageAsync(model.ImageFile);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
                await PopulateOptionsAsync(model);
                return View(model);
            }
        }

        var dto = new UpdateArticleDto
        {
            Id = model.Id,
            CategoryId = model.CategoryId,
            ImageUrl = imageUrl ?? string.Empty,
            TagIds = model.SelectedTagIds,
            IsTrending = model.IsTrending,
            Translations = BuildTranslations(model)
        };

        var updated = await _articleService.UpdateArticleAsync(dto);
        if (updated == null) return NotFound();

        TempData["Success"] = "Article updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _articleService.DeleteArticleAsync(id);
        if (!deleted) return NotFound();

        TempData["Success"] = "Article deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Publish/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        var published = await _articleService.PublishArticleAsync(id);
        if (published == null) return NotFound();

        TempData["Success"] = "Article published.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateOptionsAsync(ArticleFormViewModel model)
    {
        var categories = await _categoryService.GetCategoriesAsync(new() { Page = 1, PageSize = 1000 });
        model.CategoryOptions = categories.Items
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();

        var tags = await _tagService.GetTagsAsync(new() { Page = 1, PageSize = 1000 });
        model.TagOptions = tags.Items
            .Select(t => new SelectListItem(t.Name, t.Id.ToString()))
            .ToList();
    }

    private static ArticleTranslationViewModel MapTranslation(ArticleTranslationDto? dto, string languageCode) => new()
    {
        LanguageCode = languageCode,
        Title = dto?.Title ?? "",
        Slug = dto?.Slug ?? "",
        Content = dto?.Content ?? "",
        Excerpt = dto?.Excerpt,
        MetaTitle = dto?.MetaTitle,
        MetaDescription = dto?.MetaDescription
    };

    private static List<ArticleTranslationDto> BuildTranslations(ArticleFormViewModel model) => new()
    {
        new()
        {
            LanguageCode = "en", Title = model.English.Title, Slug = model.English.Slug,
            Content = model.English.Content, Excerpt = model.English.Excerpt,
            MetaTitle = model.English.MetaTitle, MetaDescription = model.English.MetaDescription
        },
        new()
        {
            LanguageCode = "ar", Title = model.Arabic.Title, Slug = model.Arabic.Slug,
            Content = model.Arabic.Content, Excerpt = model.Arabic.Excerpt,
            MetaTitle = model.Arabic.MetaTitle, MetaDescription = model.Arabic.MetaDescription
        }
    };

    private Guid? GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idClaim, out var id) ? id : null;
    }

    private async Task<string?> SaveImageAsync(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            throw new InvalidOperationException("Only JPG, PNG or WEBP images are allowed.");

        if (file.Length > 5 * 1024 * 1024)
            throw new InvalidOperationException("Image must be smaller than 5 MB.");

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "articles");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/articles/{fileName}";
    }
}