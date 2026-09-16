using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.DTOs.ArticleRatings;

public class ArticleRatingFilter: PaginationParams
{
    public int? ArticleId { get; set; }
}