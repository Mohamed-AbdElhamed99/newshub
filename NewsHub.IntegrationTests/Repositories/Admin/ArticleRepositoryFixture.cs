using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Data;
using NewsHub.Infrastructure.Identity;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class ArticleRepositoryFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    // Context used by the test to exercise the repository under test.
    public NewsHubDbContext Context { get; }

    public ArticleRepositoryFixture()
    {
        // ":memory:" alone would give each connection its own private DB;
        // keeping this single connection open for the fixture's lifetime is
        // what makes the in-memory SQLite DB persist across contexts/queries.
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        Context = new NewsHubDbContext(BuildOptions());
        Context.Database.EnsureCreated();
    }

    private DbContextOptions<NewsHubDbContext> BuildOptions() =>
        new DbContextOptionsBuilder<NewsHubDbContext>()
            .UseSqlite(_connection)
            .Options;

    // A second, independent context bound to the same underlying SQLite
    // connection/schema — use this to assert on what the repository
    // persisted, without reusing its change tracker (avoids false positives
    // from reading back a still-tracked in-memory entity graph).
    public NewsHubDbContext CreateAssertionContext() => new(BuildOptions());

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

    public Article SeedArticle(
        Category category,
        ApplicationUser user,
        ArticleStatus status = ArticleStatus.Draft,
        string? translationTitle = null)
    {
        var article = new Article
        {
            AuthorId = user.Id,
            CategoryId = category.Id,
            Status = status
        };

        if (translationTitle is not null)
        {
            article.Translations.Add(new ArticleTranslation
            {
                LanguageCode = "en",
                Title = translationTitle,
                Slug = translationTitle.ToLowerInvariant().Replace(' ', '-'),
                Content = "content"
            });
        }

        Context.Articles.Add(article);
        Context.SaveChanges();
        return article;
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}