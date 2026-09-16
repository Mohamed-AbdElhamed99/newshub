using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Areas.Admin.ViewModels.Articles;

public class ArticleTranslationViewModel
{
    public string LanguageCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]
    [StringLength(200)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Lowercase letters, numbers and hyphens only")]
    public string Slug { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    public string Content { get; set; } = string.Empty;

    [StringLength(500)] public string? Excerpt { get; set; }

    [StringLength(70, ErrorMessage = "Meta title should stay under 70 characters for SEO")]
    [Display(Name = "Meta Title")]
    public string? MetaTitle { get; set; }

    [StringLength(160, ErrorMessage = "Meta description should stay under 160 characters for SEO")]
    [Display(Name = "Meta Description")]
    public string? MetaDescription { get; set; }
}