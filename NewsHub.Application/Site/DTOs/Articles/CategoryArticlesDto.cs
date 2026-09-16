namespace NewsHub.Application.Site.DTOs.Articles;

public class CategoryArticlesDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<LatestArticleDto> Articles { get; set; } = new();
}