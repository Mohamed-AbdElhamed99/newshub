using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Identity;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class ArticleRatingRepositoryFixture : SqliteInMemoryFixtureBase
{
    public Category SeedCategory()
    {
        var category = new Category();
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

    public Article SeedArticle(Category category, ApplicationUser user)
    {
        var article = new Article
        {
            AuthorId = user.Id,
            CategoryId = category.Id,
            Status = ArticleStatus.Published
        };
        Context.Articles.Add(article);
        Context.SaveChanges();
        return article;
    }
}
