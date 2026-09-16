using NewsHub.Domain.Entities;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class CategoryRepositoryFixture : SqliteInMemoryFixtureBase
{
    public Category SeedCategory(string? translationName = null)
    {
        var category = new Category();

        if (translationName is not null)
        {
            category.Translations.Add(new CategoryTranslation
            {
                LanguageCode = "en",
                Name = translationName,
                Slug = translationName.ToLowerInvariant().Replace(' ', '-')
            });
        }

        Context.Categories.Add(category);
        Context.SaveChanges();
        return category;
    }
}