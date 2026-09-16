using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Areas.Admin.ViewModels.Tags;

public class TagTranslationViewModel
{
    public string LanguageCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Slug is required")]
    [StringLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Lowercase letters, numbers and hyphens only")]
    public string Slug { get; set; } = string.Empty;
}