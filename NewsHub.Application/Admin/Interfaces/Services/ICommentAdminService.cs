using NewsHub.Application.Admin.DTOs.Comments;
using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.Interfaces.Services;

public interface ICommentAdminService
{
    Task<PagedResult<CommentAdminDto>> GetCommentsAsync(CommentFilter filter);
    Task<CommentAdminDto?> ApproveCommentAsync(int id);
    Task<CommentAdminDto?> RejectCommentAsync(int id);
    Task<bool> DeleteCommentAsync(int id);
}