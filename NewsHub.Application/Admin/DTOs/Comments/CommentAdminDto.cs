using NewsHub.Domain.Enums;

namespace NewsHub.Application.Admin.DTOs.Comments;

public class CommentAdminDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public CommentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}