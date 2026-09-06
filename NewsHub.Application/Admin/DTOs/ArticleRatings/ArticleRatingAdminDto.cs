namespace NewsHub.Application.Admin.DTOs.ArticleRatings;

public class ArticleRatingAdminDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}