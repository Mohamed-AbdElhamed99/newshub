namespace NewsHub.Application.Site.DTOs;

public class TopStoryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}