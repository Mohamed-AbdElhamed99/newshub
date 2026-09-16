using NewsHub.Domain.Entities;

namespace NewsHub.Web.Extensions;

public static class TranslationExtensions
{
    private const string FallbackLanguageCode = "en";

    public static ArticleTranslation GetTranslation(this Article article, string languageCode)
    {
        return article.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
               ?? article.Translations.FirstOrDefault(t => t.LanguageCode == FallbackLanguageCode)
               ?? article.Translations.First();
    }

    public static CategoryTranslation GetTranslation(this Category category, string languageCode)
    {
        return category.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
               ?? category.Translations.FirstOrDefault(t => t.LanguageCode == FallbackLanguageCode)
               ?? category.Translations.First();
    }

    public static TagTranslation GetTranslation(this Tag tag, string languageCode)
    {
        return tag.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
               ?? tag.Translations.FirstOrDefault(t => t.LanguageCode == FallbackLanguageCode)
               ?? tag.Translations.First();
    }

    // Convenience overload so views can just call article.GetTranslation()
    // and pick up the current request's UI culture automatically.
    public static ArticleTranslation GetTranslation(this Article article) =>
        article.GetTranslation(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

    public static CategoryTranslation GetTranslation(this Category category) =>
        category.GetTranslation(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

    public static TagTranslation GetTranslation(this Tag tag) =>
        tag.GetTranslation(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
}