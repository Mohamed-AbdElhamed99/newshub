using NewsHub.Domain.Entities;

namespace NewsHub.Domain.BusinessRules;

public class ConsistencyRules
{
    public static bool HasNoOrphanedTranslations(IEnumerable<ArticleTranslation> translations)
    {
        if (translations == null) return true;

        return translations.All(t => t.ArticleId > 0);
    }
}