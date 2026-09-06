using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface IArticleRepository
{
    Task<Article?> GetByIdAsync(int id);
    Task<Article?> GetBySlugAsync(string slug);

    Task<IEnumerable<Article>> GetTrendingAsync(int count);
    Task<IEnumerable<ArticleWithCommentCount>> GetLatestAsync(int count);
    Task<Article?> GetTopStoryAsync();
    Task<IEnumerable<ArticleWithCommentCount>> GetMostViewedAsync(int count);
    Task<IEnumerable<ArticleWithCommentCount>> GetByCategoryAsync(int categoryId, int count);
    Task<IEnumerable<ArticleWithRating>> GetHighestRatedAsync(int count);
    Task<(IEnumerable<ArticleWithCommentCount> Items, int TotalCount)> GetPagedAsync(ArticleListFilter filter);
    Task<int> GetApprovedCommentCountAsync(int articleId); // single-article count, needed by article details page

    Task IncrementViewCountAsync(int articleId);
    Task UpdateAsync(Article article);
    Task<bool> ExistsAsync(int id);
}