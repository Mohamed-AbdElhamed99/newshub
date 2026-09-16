namespace NewsHub.Application.Site.DTOs.Articles;

public class PopularArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string LanguageCode { get; set; } = string.Empty;
}