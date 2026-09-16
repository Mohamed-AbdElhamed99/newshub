namespace NewsHub.Application.Site.DTOs.Tags;

public class TrendingTagDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}