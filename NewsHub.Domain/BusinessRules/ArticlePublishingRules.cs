using NewsHub.Domain.Entities;

namespace NewsHub.Domain.BusinessRules;

public static class ArticlePublishingRules
{
    public static bool CanPublish(Article article)
    {
        return HasRequiredTranslations(article) && HasImageUrl(article) && HasAuthor(article) && HasCategory(article);
    }

    private static bool HasRequiredTranslations(Article article) => 
        article.Translations.Any(t => t.LanguageCode == "ar" && !string.IsNullOrWhiteSpace(t.Title) && !string.IsNullOrWhiteSpace(t.Content)) && 
        article.Translations.Any(t => t.LanguageCode == "en" && !string.IsNullOrWhiteSpace(t.Title) && !string.IsNullOrWhiteSpace(t.Content));
    
    private static bool HasImageUrl(Article article) => !string.IsNullOrWhiteSpace(article.ImageUrl);
    
    private static bool HasAuthor(Article article) => !string.IsNullOrWhiteSpace(article.AuthorId);
    
    private static bool HasCategory(Article article) => article.CategoryId > 0;
}