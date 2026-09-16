using NewsHub.Domain.Enums;

namespace NewsHub.Application.Admin.DTOs.Articles;

public class ArticleAdminDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public ArticleStatus Status { get; set; }
    public bool IsTrending { get; set; }
    public int ViewCount { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
}