namespace NewsHub.Application.Admin.DTOs.SiteSettings;

public class UpdateSiteSettingDto
{
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactAddress { get; set; } = string.Empty;
}