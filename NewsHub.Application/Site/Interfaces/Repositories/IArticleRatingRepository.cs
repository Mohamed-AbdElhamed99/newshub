using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface IArticleRatingRepository
{
    Task AddAsync(ArticleRating article);
    Task<ArticleRating?> GetByArticleAndUserAsync(int  articleId, Guid userId);
    Task UpdateAsync (ArticleRating article);
}