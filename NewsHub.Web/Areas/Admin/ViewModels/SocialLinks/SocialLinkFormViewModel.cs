namespace NewsHub.Web.Areas.Admin.ViewModels.SocialLinks;

using System.ComponentModel.DataAnnotations;

public class SocialLinkFormViewModel
{
    public int? Id { get; set; }

    [Required, Display(Name = "Platform")]
    public string Platform { get; set; } = string.Empty;

    [Required, Url, Display(Name = "URL")]
    public string Url { get; set; } = string.Empty;

    [Display(Name = "Icon Class")]
    public string? IconClass { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}