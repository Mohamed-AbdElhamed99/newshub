namespace NewsHub.Application.Admin.DTOs.Articles;

public class ArticleTranslationDto
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}