namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Dictionary<int, int>> GetApprovedCommentCountsAsync(IEnumerable<int> articleIds);

}