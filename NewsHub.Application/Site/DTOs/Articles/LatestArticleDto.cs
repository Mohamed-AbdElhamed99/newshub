namespace NewsHub.Application.Site.DTOs.Articles;

public class LatestArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public int CommentCount { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}