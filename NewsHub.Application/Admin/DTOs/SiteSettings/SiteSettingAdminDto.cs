namespace NewsHub.Application.Admin.DTOs.SiteSettings;

public class SiteSettingAdminDto
{
    public int Id { get; set; }
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactAddress { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}