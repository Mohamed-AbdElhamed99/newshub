using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Enums;

namespace NewsHub.Application.Admin.DTOs.Articles;

public class ArticleFilter : PaginationParams
{
    public int? CategoryId { get; set; }
    public ArticleStatus? Status { get; set; }
    public string? SearchTerm { get; set; }
}