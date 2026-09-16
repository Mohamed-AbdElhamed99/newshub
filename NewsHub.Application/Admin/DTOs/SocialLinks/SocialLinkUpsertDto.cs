namespace NewsHub.Application.Admin.DTOs.SocialLinks;

public class SocialLinkUpsertDto
{
    public int? Id { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? IconClass { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}