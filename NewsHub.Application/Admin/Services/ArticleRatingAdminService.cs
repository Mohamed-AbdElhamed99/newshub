using NewsHub.Application.Admin.DTOs.ArticleRatings;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Services;

public class ArticleRatingAdminService : IArticleRatingAdminService
{
    private readonly IArticleRatingRepository _repository;

    public ArticleRatingAdminService(IArticleRatingRepository repository)
    {
        this._repository = repository;
    }

    public async Task<PagedResult<ArticleRatingAdminDto>> GetRatingsAsync(ArticleRatingFilter filter)
    {
        var (ratings, totalCount) = await _repository.GetAllAsync(filter);

        var items = ratings.Select(MapToDto).ToList();

        return new PagedResult<ArticleRatingAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ArticleRatingSummaryDto> GetSummaryByArticleIdAsync(int articleId)
    {
        var (average, totalRatings) = await _repository.GetSummaryByArticleIdAsync(articleId);

        return new ArticleRatingSummaryDto
        {
            ArticleId = articleId,
            AverageRating = average,
            TotalRatings = totalRatings
        };
    }

    public async Task<bool> DeleteRatingAsync(int id)
    {
        var rating = await _repository.GetByIdAsync(id);
        if (rating == null) return false;

        await _repository.DeleteAsync(rating);
        return true;
    }

    private static ArticleRatingAdminDto MapToDto(ArticleRating rating) =>
        new ArticleRatingAdminDto
        {
            Id = rating.Id,
            ArticleId = rating.ArticleId,
            UserId = rating.UserId,
            Rating = rating.Rating,
            CreatedAt = rating.CreatedAt,
            UpdatedAt = rating.UpdatedAt
        };
}