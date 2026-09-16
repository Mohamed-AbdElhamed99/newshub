namespace NewsHub.Application.Admin.DTOs.Articles;

public class UpdateArticleDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsTrending { get; set; }
    public List<int> TagIds { get; set; } = new();
    public List<ArticleTranslationDto> Translations { get; set; } = new();
}