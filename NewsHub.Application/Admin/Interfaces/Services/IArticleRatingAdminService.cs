using NewsHub.Application.Admin.DTOs.ArticleRatings;
using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.Interfaces.Services;

public interface IArticleRatingAdminService
{
    Task<PagedResult<ArticleRatingAdminDto>> GetRatingsAsync(ArticleRatingFilter filter);
    Task<ArticleRatingSummaryDto> GetSummaryByArticleIdAsync(int articleId);
    Task<bool> DeleteRatingAsync(int id);
}
