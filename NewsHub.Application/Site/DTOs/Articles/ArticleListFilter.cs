using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Site.DTOs.Articles;

public class ArticleListFilter : PaginationParams
{
    public int? CategoryId { get; set; }
    public int? TagId { get; set; }
    public string? SearchTerm { get; set; }
}