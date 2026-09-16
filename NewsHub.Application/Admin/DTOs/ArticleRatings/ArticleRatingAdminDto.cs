namespace NewsHub.Application.Admin.DTOs.ArticleRatings;

public class ArticleRatingAdminDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public Guid UserId { get; set; } = default;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}