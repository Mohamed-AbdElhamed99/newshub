using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(int id);
    Task<Article?> GetBySlugAsync(string slug);

    Task<IEnumerable<Article>> GetTrendingAsync(int count);
    Task<IEnumerable<Article>> GetLatestAsync(int count);
    Task<Article?> GetTopStoryAsync();
    Task<IEnumerable<Article>> GetMostViewedAsync(int count);
    Task<IEnumerable<Article>> GetByCategoryAsync(int categoryId, int count);

    Task IncrementViewCountAsync(int articleId);
}