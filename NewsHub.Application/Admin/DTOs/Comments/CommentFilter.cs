using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Enums;

namespace NewsHub.Application.Admin.DTOs.Comments;

public class CommentFilter: PaginationParams
{
    public int? ArticleId { get; set; }
    public CommentStatus? Status { get; set; }
}