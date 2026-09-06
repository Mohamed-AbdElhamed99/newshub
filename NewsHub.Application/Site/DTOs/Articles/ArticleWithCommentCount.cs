using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.DTOs.Articles;

public class ArticleWithCommentCount
{
    public Article Article { get; set; } = null!;
    public int CommentCount { get; set; }
}