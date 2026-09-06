using NewsHub.Domain.Entities;

namespace NewsHub.Domain.BusinessRules;

public static class CommentRules
{
    public static bool CanAdd(Comment comment) =>
        comment.UserId != Guid.Empty &&
        comment.ArticleId > 0 &&
        !string.IsNullOrWhiteSpace(comment.Content);

    public static bool CanModify(Comment comment, Guid userId) => comment.UserId == userId;
}