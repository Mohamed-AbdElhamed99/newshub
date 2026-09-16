using NewsHub.Application.Admin.DTOs.ArticleRatings;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Interfaces.Repositories;

public interface IArticleRatingRepository
{
    Task<ArticleRating?> GetByIdAsync(int id);
    Task<(List<ArticleRating> Items, int TotalCount)> GetAllAsync(ArticleRatingFilter filter);
    Task<(double Average, int TotalRatings)> GetSummaryByArticleIdAsync(int articleId);
    Task DeleteAsync(ArticleRating rating);
}