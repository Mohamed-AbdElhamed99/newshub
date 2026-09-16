using NewsHub.Application.Site.DTOs.Tags;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface ITagService
{
    Task<IEnumerable<TrendingTagDto>> GetTrendingTagsAsync(int count);
}