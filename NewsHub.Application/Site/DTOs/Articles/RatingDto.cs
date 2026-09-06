namespace NewsHub.Application.Site.DTOs.Articles;

public class RatingDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int ArticleId { get; set; }
    public int Value { get; set; }
}