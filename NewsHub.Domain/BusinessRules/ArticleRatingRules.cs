using NewsHub.Domain.Entities;

namespace NewsHub.Domain.BusinessRules;

public static class ArticleRatingRules
{
    public static bool CanRate(ArticleRating rating , IEnumerable<ArticleRating> ratings)
    {
        return IsWithinRange(rating) && IsUnique(rating ,  ratings);
    }
    
    private static bool IsWithinRange(ArticleRating rating) => rating.Rating >= 1 && rating.Rating <= 5;
    
    private static bool IsUnique(ArticleRating rating, IEnumerable<ArticleRating> ratings)
        => !ratings.Any(r => r.UserId == rating.UserId &&  r.ArticleId == rating.ArticleId);
}