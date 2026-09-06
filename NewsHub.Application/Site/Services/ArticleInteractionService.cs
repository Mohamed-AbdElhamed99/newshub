using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.BusinessRules;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Domain.Exceptions;

namespace NewsHub.Application.Site.Services;

public class ArticleInteractionService : IArticleInteractionService
{
    private readonly IArticleRepository _articles;
    private readonly IArticleRatingRepository _ratingRepository;
    private readonly ICommentRepository _comments;
    private readonly IUserService _userService;

    public ArticleInteractionService(IArticleRepository articles, IArticleRatingRepository ratingRepository,
        ICommentRepository comment , IUserService userService)
    {
        _articles = articles;
        _ratingRepository = ratingRepository;
        _comments = comment;
        _userService = userService;
    }

    public async Task<CommentDto> AddCommentAsync(CommentDto dto)
    {
        if (!await _articles.ExistsAsync(dto.ArticleId))
            throw new NotFoundException(nameof(Article), dto.ArticleId);
        
        if (!await _userService.IsExistsAsync(dto.UserId))
            throw new NotFoundException("User not found");

        var comment = new Comment
        {
            ArticleId = dto.ArticleId,
            UserId = dto.UserId,
            Content = dto.Content?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            Status = CommentStatus.Pending
        };
        
        if (!CommentRules.CanAdd(comment))
            throw new ArgumentException(
                "Invalid comment: content must not be empty, and the comment must reference a valid article and user.");

        await _comments.AddAsync(comment);

        return new CommentDto
        {
            Id = comment.Id,
            ArticleId = comment.ArticleId,
            UserId = comment.UserId,
            Content = comment.Content
        };
    }

    public async Task<CommentDto> UpdateCommentAsync(int commentId, Guid userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty.");

        var comment = await _comments.GetByIdAsync(commentId);
        if (comment is null)
            throw new NotFoundException(nameof(Comment), commentId);

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You can only edit your own comment.");

        comment.Content = content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
        comment.Status = CommentStatus.Pending; // re-moderate on edit — confirm this is what you want

        await _comments.UpdateAsync(comment);

        return new CommentDto
        {
            Id = comment.Id,
            ArticleId = comment.ArticleId,
            UserId = comment.UserId,
            Content = comment.Content
        };
    }

    public async Task DeleteCommentAsync(int commentId, Guid userId)
    {
        var comment = await _comments.GetByIdAsync(commentId);
        if (comment is null)
            throw new NotFoundException(nameof(Comment), commentId);

        if (comment.UserId != userId)
            throw new UnauthorizedAccessException("You can only delete your own comment.");

        await _comments.DeleteAsync(comment);
    }

    public async Task<IEnumerable<CommentDto>> GetUserCommentsForArticleAsync(int articleId, Guid userId)
    {
        var comments = await _comments.GetByArticleAndUserAsync(articleId, userId);

        return comments.Select(c => new CommentDto
        {
            Id = c.Id,
            ArticleId = c.ArticleId,
            UserId = c.UserId,
            Content = c.Content
        });
    }

    public async Task<RatingDto> RateArticleAsync(RatingDto dto)
    {
        if (dto.Value < 1 || dto.Value > 5)
            throw new ArgumentException("Rating must be between 1 and 5");

        if (!await _articles.ExistsAsync(dto.ArticleId))
            throw new NotFoundException($"Article {dto.ArticleId} not found");

        var existing = await _ratingRepository.GetByArticleAndUserAsync(dto.ArticleId, dto.UserId);
        if (existing is not null)
        {
            existing.Rating = dto.Value;
            existing.UpdatedAt = DateTime.UtcNow;
            await _ratingRepository.UpdateAsync(existing);
            return new RatingDto
            {
                Id = existing.Id, ArticleId = existing.ArticleId, UserId = existing.UserId, Value = existing.Rating
            };
        }

        var rating = new ArticleRating
        {
            ArticleId = dto.ArticleId,
            UserId = dto.UserId,
            Rating = dto.Value,
            CreatedAt = DateTime.UtcNow
        };
        await _ratingRepository.AddAsync(rating);
        return new RatingDto
            { Id = rating.Id, ArticleId = rating.ArticleId, UserId = rating.UserId, Value = rating.Rating };
    }
}