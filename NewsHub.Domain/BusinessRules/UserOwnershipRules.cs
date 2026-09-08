using NewsHub.Domain.Entities;

namespace NewsHub.Domain.BusinessRules;

public static class UserOwnershipRules
{
    public static bool IsUserOwnArticle(Article article, Guid userId)
    {
        if (article == null) return false;
        if (article.AuthorId == Guid.Empty) return false;
        if (userId == Guid.Empty) return false;

        return  article.AuthorId.Equals(userId);
    }
}