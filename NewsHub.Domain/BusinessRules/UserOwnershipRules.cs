using NewsHub.Domain.Entities;

namespace NewsHub.Domain.BusinessRules;

public static class UserOwnershipRules
{
    public static bool IsUserOwnArticle(Article article, string userId)
    {
        if (article == null) return false;
        if (string.IsNullOrWhiteSpace(article.AuthorId)) return false;
        if (string.IsNullOrWhiteSpace(userId)) return false;

        return article.AuthorId == userId;
    }
}