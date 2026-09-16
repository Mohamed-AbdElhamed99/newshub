namespace NewsHub.Domain.Entities;

public class ArticleTranslation
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
   
}