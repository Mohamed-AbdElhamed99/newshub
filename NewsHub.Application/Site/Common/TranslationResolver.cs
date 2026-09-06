using System.Globalization;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Common;

public static class TranslationResolver
{
    public static string ResolveName(IEnumerable<TagTranslation> translations, out string languageCode)
    {
        var translation = translations
                              .FirstOrDefault(t => t.LanguageCode == CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
                          ?? translations.FirstOrDefault(t => t.LanguageCode == "en")
                          ?? translations.First();

        languageCode = translation.LanguageCode;
        return translation.Name;
    }
}