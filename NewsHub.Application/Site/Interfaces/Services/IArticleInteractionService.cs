using NewsHub.Application.Site.DTOs.Articles;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IArticleInteractionService
{
    Task<CommentDto> AddCommentAsync(CommentDto dto);
    Task<RatingDto> RateArticleAsync(RatingDto dto);
}