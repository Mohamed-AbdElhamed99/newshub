using NewsHub.Domain.Entities;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class TagRepositoryFixture : SqliteInMemoryFixtureBase
{
    public Tag SeedTag(string? translationName = null)
    {
        var tag = new Tag();

        if (translationName is not null)
        {
            tag.Translations.Add(new TagTranslation
            {
                LanguageCode = "en",
                Name = translationName,
                Slug = translationName.ToLowerInvariant().Replace(' ', '-')
            });
        }

        Context.Tags.Add(tag);
        Context.SaveChanges();
        return tag;
    }
}