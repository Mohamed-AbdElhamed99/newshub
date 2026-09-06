using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Dictionary<int, int>> GetApprovedCommentCountsAsync(IEnumerable<int> articleIds);
    Task AddAsync(Comment comment);
    Task DeleteAsync(Comment comment);
    Task UpdateAsync(Comment comment);
    Task<Comment?> GetByIdAsync(int id);
    Task<IEnumerable<Comment>> GetByArticleAndUserAsync(int articleId, Guid userId);
}