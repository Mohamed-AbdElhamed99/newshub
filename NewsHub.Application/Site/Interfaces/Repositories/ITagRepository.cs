using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface ITagRepository
{
    Task<IEnumerable<Tag>> GetTrendingTagsAsync(int count);
    Task<IEnumerable<Tag>> GetByIdsAsync(IEnumerable<int> tagIds);
}