using NewsHub.Domain.Enums;

namespace NewsHub.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public CommentStatus Status { get; set; } = CommentStatus.Pending; 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}