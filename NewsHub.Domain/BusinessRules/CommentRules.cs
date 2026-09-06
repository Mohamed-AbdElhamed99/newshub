using NewsHub.Domain.Entities;

namespace NewsHub.Domain.BusinessRules;

public static class CommentRules
{
    public static bool CanAdd(Comment comment) =>
        comment.ArticleId > 0 &&
        !string.IsNullOrWhiteSpace(comment.UserId) &&
        !string.IsNullOrWhiteSpace(comment.Content);

    public static bool CanModify(Comment comment, string userId) => comment.UserId == userId;
}