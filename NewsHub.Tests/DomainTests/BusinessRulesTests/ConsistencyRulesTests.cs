using NewsHub.Domain.BusinessRules;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.DomainTests.BusinessRulesTests;

public class ConsistencyRulesTests
{
    [Fact]
    public void HasNoOrphanedTranslations_ReturnTrue_WhenAllTranslationsHaveArticleId()
    {
        var translations = new List<ArticleTranslation>
        {
            new ArticleTranslation { Id = 1, ArticleId = 1, LanguageCode = "en", Title = "Breaking News", Content = "Some text" },
            new ArticleTranslation { Id = 2, ArticleId = 1, LanguageCode = "ar", Title = "أخبار عاجلة", Content = "النص" }
        };

        var result = ConsistencyRules.HasNoOrphanedTranslations(translations);

        Assert.True(result);
    }

    [Fact]
    public void HasNoOrphanedTranslations_ReturnFalse_WhenAnyTranslationHasNoArticleId()
    {
        var translations = new List<ArticleTranslation>
        {
            new ArticleTranslation { Id = 1, ArticleId = 0, LanguageCode = "en", Title = "Breaking News", Content = "Some text" },
            new ArticleTranslation { Id = 2, ArticleId = 1, LanguageCode = "ar", Title = "أخبار عاجلة", Content = "النص" }
        };

        var result = ConsistencyRules.HasNoOrphanedTranslations(translations);

        Assert.False(result);
    }

    [Fact]
    public void HasNoOrphanedTranslations_ReturnTrue_WhenTranslationsListIsEmpty()
    {
        var translations = new List<ArticleTranslation>();

        var result = ConsistencyRules.HasNoOrphanedTranslations(translations);

        Assert.True(result);
    }
}