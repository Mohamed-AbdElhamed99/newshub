using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Identity;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class ArticleRepositoryFixture : SqliteInMemoryFixtureBase
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

    public Article SeedArticle(
        Category category,
        ApplicationUser user,
        ArticleStatus status = ArticleStatus.Published,
        bool isTrending = false,
        int viewCount = 0,
        string? title = null,
        DateTime? publishedAt = null)
    {
        var article = new Article
        {
            AuthorId = user.Id,
            CategoryId = category.Id,
            Status = status,
            IsTrending = isTrending,
            ViewCount = viewCount,
            PublishedAt = publishedAt ?? DateTime.UtcNow
        };

        if (title is not null)
        {
            article.Translations.Add(new ArticleTranslation
            {
                LanguageCode = "en",
                Title = title,
                Slug = title.ToLowerInvariant().Replace(' ', '-'),
                Content = "content"
            });
        }

        Context.Articles.Add(article);
        Context.SaveChanges();
        return article;
    }

    public Comment SeedComment(Article article, CommentStatus status = CommentStatus.Approved, Guid? userId = null)
    {
        var comment = new Comment
        {
            ArticleId = article.Id,
            UserId = userId ?? Guid.NewGuid(),
            Content = "Sample comment",
            Status = status
        };
        Context.Comments.Add(comment);
        Context.SaveChanges();
        return comment;
    }

    public ArticleRating SeedRating(Article article, int rating, Guid? userId = null)
    {
        var articleRating = new ArticleRating
        {
            ArticleId = article.Id,
            UserId = userId ?? Guid.NewGuid(),
            Rating = rating
        };
        Context.ArticleRatings.Add(articleRating);
        Context.SaveChanges();
        return articleRating;
    }

    public Tag SeedTag(string? name = null)
    {
        var tag = new Tag();

        if (name is not null)
        {
            tag.Translations.Add(new TagTranslation
            {
                LanguageCode = "en",
                Name = name,
                Slug = name.ToLowerInvariant().Replace(' ', '-')
            });
        }

        Context.Tags.Add(tag);
        Context.SaveChanges();
        return tag;
    }

    public void AttachTag(Article article, Tag tag)
    {
        Context.ArticleTags.Add(new ArticleTag { ArticleId = article.Id, TagId = tag.Id });
        Context.SaveChanges();
    }
}
