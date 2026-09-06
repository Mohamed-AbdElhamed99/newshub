using NewsHub.Domain.Enums;

namespace NewsHub.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public CommentStatus Status { get; set; } = CommentStatus.Pending; 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}