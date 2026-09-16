namespace NewsHub.Web.Areas.Admin.ViewModels.SiteSettings;

using System.ComponentModel.DataAnnotations;

public class SiteSettingFormViewModel
{
    [Required, EmailAddress, Display(Name = "Contact Email")]
    public string ContactEmail { get; set; } = string.Empty;

    [Required, Display(Name = "Contact Phone")]
    public string ContactPhone { get; set; } = string.Empty;

    [Required, Display(Name = "Contact Address")]
    public string ContactAddress { get; set; } = string.Empty;
}