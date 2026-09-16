using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.DTOs.Tags;

namespace NewsHub.Web.Models.Article;

public class ArticleDetailsViewModel
{
    public ArticleDetailDto Article { get; set; } = null!;
    public IEnumerable<CommentDto> Comments { get; set; } = Enumerable.Empty<CommentDto>();
    public CommentFormViewModel CommentForm { get; set; } = new();
    public RatingFormViewModel RatingForm { get; set; } = new();
    public bool CanInteract { get; set; }
    public IEnumerable<PopularArticleDto> PopularNews { get; set; } = Enumerable.Empty<PopularArticleDto>();
    public IEnumerable<TrendingTagDto> TrendingTags { get; set; } = Enumerable.Empty<TrendingTagDto>();
}