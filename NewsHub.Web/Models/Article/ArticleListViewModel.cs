using NewsHub.Application.Common.Pagination;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Web.Models.Shared;

namespace NewsHub.Web.Models.Article;

public class ArticleListViewModel
{
    public PagedResult<LatestArticleDto> Result { get; set; } = null!;
    public string Heading { get; set; } = "Latest News";
    public string? SearchTerm { get; set; }
    public PaginationViewModel Pagination { get; set; } = null!;
}