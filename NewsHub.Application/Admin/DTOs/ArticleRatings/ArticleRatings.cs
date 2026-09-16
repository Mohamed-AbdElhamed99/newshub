namespace NewsHub.Application.Admin.DTOs.ArticleRatings;

public class ArticleRatings
{
    public int ArticleId { get; set; }
    public double AverageRating { get; set; }
    public int TotalRatings { get; set; }
}