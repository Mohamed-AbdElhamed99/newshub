
namespace NewsHub.Web.Areas.Admin.ViewModels.Tags;

public class TagFormViewModel
{
    public int Id { get; set; }

    public TagTranslationViewModel English { get; set; } = new() { LanguageCode = "en" };
    public TagTranslationViewModel Arabic { get; set; } = new() { LanguageCode = "ar" };
}