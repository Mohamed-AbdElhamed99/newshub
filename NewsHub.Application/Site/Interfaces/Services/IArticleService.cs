using NewsHub.Application.Common.Pagination;
using NewsHub.Application.Site.DTOs;
using NewsHub.Application.Site.DTOs.Articles;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IArticleService
{
    Task<IEnumerable<TrendingArticleDto>> GetTrendingAsync(int count);
    Task<IEnumerable<LatestArticleDto>> GetLatestAsync(int count);
    Task<TopStoryDto?> GetTopStoryAsync();
    Task<IEnumerable<LatestArticleDto>> GetMostViewedAsync(int count);
    Task<IEnumerable<PopularArticleDto>> GetPopularAsync(int count);
    Task<CategoryArticlesDto> GetByCategoryAsync(int categoryId, string categoryName, int count);
    Task<ArticleDetailDto?> GetArticleDetailBySlugAsync(string slug);
    Task<PagedResult<LatestArticleDto>> GetPagedAsync(ArticleListFilter filter);
    Task RegisterViewAsync(int articleId);
}