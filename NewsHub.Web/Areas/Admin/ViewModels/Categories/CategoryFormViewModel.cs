using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Areas.Admin.ViewModels.Categories;

public class CategoryFormViewModel
{
    public int Id { get; set; }

    public string? ExistingImagePath { get; set; }

    [Display(Name = "Category Image")]
    public IFormFile? ImageFile { get; set; }

    public CategoryTranslationViewModel English { get; set; } = new() { LanguageCode = "en" };
    public CategoryTranslationViewModel Arabic { get; set; } = new() { LanguageCode = "ar" };
}