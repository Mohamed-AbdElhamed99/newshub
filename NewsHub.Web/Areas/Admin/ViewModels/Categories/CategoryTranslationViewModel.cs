using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Areas.Admin.ViewModels.Categories;

public class CategoryTranslationViewModel
{
    public string LanguageCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]
    [StringLength(150)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        ErrorMessage = "Slug must be lowercase letters, numbers and hyphens only (e.g. sports-news)")]
    public string Slug { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description can't exceed 500 characters")]
    public string? Description { get; set; }
}