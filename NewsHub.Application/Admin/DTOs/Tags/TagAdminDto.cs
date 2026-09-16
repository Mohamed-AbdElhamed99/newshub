namespace NewsHub.Application.Admin.DTOs.Tags;

public class TagAdminDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}