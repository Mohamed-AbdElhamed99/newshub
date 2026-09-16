using NewsHub.Application.Admin.DTOs.Comments;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(int id);
    Task<(List<Comment> Items, int TotalCount)> GetAllAsync(CommentFilter filter);
    Task UpdateAsync(Comment comment);
    Task DeleteAsync(Comment comment);
}