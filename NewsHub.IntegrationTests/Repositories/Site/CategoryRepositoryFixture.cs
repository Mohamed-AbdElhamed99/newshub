using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Identity;

namespace NewsHub.IntegrationTests.Repositories.Site;

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

    public ApplicationUser SeedUser()
    {
        var user = new ApplicationUser
        {
            UserName = $"user_{Guid.NewGuid():N}",
            Email = $"{Guid.NewGuid():N}@test.com"
        };
        Context.Users.Add(user);
        Context.SaveChanges();
        return user;
    }

    public void SeedPublishedArticle(Category category, ApplicationUser user)
    {
        Context.Articles.Add(new Article
        {
            AuthorId = user.Id,
            CategoryId = category.Id,
            Status = ArticleStatus.Published
        });
        Context.SaveChanges();
    }
}
