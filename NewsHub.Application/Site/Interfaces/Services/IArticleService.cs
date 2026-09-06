using NewsHub.Application.Site.DTOs;
using NewsHub.Application.Site.DTOs.Articles;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IArticleService
{
    Task<IEnumerable<TrendingArticleDto>> GetTrendingAsync(int count);
    Task<IEnumerable<LatestArticleDto>> GetLatestAsync(int count);
    Task<TopStoryDto?> GetTopStoryAsync();
    Task<IEnumerable<LatestArticleDto>> GetMostViewedAsync(int count);
}