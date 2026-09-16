namespace NewsHub.Application.Site.DTOs.Articles;

public class CommentDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int ArticleId { get; set; }
    public string Content { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}