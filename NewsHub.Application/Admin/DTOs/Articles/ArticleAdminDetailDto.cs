using NewsHub.Domain.Enums;

namespace NewsHub.Application.Admin.DTOs.Articles;

public class ArticleAdminDetailDto
{
    public int Id { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public ArticleStatus Status { get; set; }
    public bool IsTrending { get; set; }
    public int ViewCount { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime PublishedAt { get; set; }
    public List<int> TagIds { get; set; } = new();
    public List<ArticleTranslationDto> Translations { get; set; } = new();
}