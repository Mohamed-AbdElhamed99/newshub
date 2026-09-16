using Bogus;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace NewsHub.Infrastructure.Seeding;

public static class DataFactories
{
    public static List<Category> GenerateCategories(int count = 5)
    {
        var categoryFaker = new Faker<Category>()
            .RuleFor(c => c.ImagePath, f => f.Image.PicsumUrl())
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1))
            .RuleFor(c => c.UpdatedAt, (f, c) => c.CreatedAt)
            .RuleFor(c => c.Translations, f =>
            {
                var categoryName = f.Commerce.Categories(1)[0];
                return new List<CategoryTranslation>
                {
                    new()
                    {
                        LanguageCode = "en",
                        Name = categoryName,
                        Description = f.Lorem.Sentence(8),
                        Slug = f.Lorem.Slug()
                    },
                    new()
                    {
                        LanguageCode = "ar",
                        Name = $"{categoryName} (AR)",
                        Description = f.Lorem.Sentence(8),
                        Slug = f.Lorem.Slug()
                    }
                };
            });

        return categoryFaker.Generate(count);
    }

    public static List<Tag> GenerateTags(int count = 10)
    {
        var tagFaker = new Faker<Tag>()
            .RuleFor(t => t.CreatedAt, f => f.Date.Past(1))
            .RuleFor(t => t.UpdatedAt, (f, t) => t.CreatedAt)
            .RuleFor(t => t.Translations, f =>
            {
                var tagName = f.Commerce.ProductAdjective();
                return new List<TagTranslation>
                {
                    new()
                    {
                        LanguageCode = "en",
                        Name = tagName,
                        Slug = f.Lorem.Slug()
                    },
                    new()
                    {
                        LanguageCode = "ar",
                        Name = $"{tagName} (AR)",
                        Slug = f.Lorem.Slug()
                    }
                };
            });

        return tagFaker.Generate(count);
    }

    public static List<Article> GenerateArticles(List<Category> categories, List<Guid> authorIds, int count = 20)
    {
        var categoryIds = categories.Select(c => c.Id).ToList();

        var articleFaker = new Faker<Article>()
            .RuleFor(a => a.AuthorId, f => f.PickRandom(authorIds))
            .RuleFor(a => a.CategoryId, f => f.PickRandom(categoryIds))
            .RuleFor(a => a.Status, f => f.PickRandom<ArticleStatus>())
            .RuleFor(a => a.IsTrending, f => f.Random.Bool(0.2f))
            .RuleFor(a => a.ViewCount, f => f.Random.Number(10, 10000))
            .RuleFor(a => a.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(a => a.CreatedAt, f => f.Date.Past(1))
            .RuleFor(a => a.UpdatedAt, (f, a) => a.CreatedAt)
            .RuleFor(a => a.PublishedAt, (f, a) => a.CreatedAt.AddDays(f.Random.Number(1, 5)))
            .RuleFor(a => a.Translations, f =>
            {
                var titleEn = f.Lorem.Sentence(5);
                var titleAr = f.Lorem.Sentence(5);
                return new List<ArticleTranslation>
                {
                    new()
                    {
                        LanguageCode = "en",
                        Title = titleEn,
                        Slug = f.Lorem.Slug(),
                        Content = f.Lorem.Paragraphs(4),
                        Excerpt = f.Lorem.Sentence(10),
                        MetaTitle = titleEn,
                        MetaDescription = f.Lorem.Sentence(12)
                    },
                    new()
                    {
                        LanguageCode = "ar",
                        Title = $"{titleAr} (AR)",
                        Slug = f.Lorem.Slug(),
                        Content = f.Lorem.Paragraphs(4),
                        Excerpt = f.Lorem.Sentence(10),
                        MetaTitle = titleAr,
                        MetaDescription = f.Lorem.Sentence(12)
                    }
                };
            });

        return articleFaker.Generate(count);
    }

    public static List<ArticleTag> GenerateArticleTags(List<int> articleIds, List<int> tagIds)
    {
        var articleTags = new List<ArticleTag>();
        var faker = new Faker();

        foreach (var articleId in articleIds)
        {
            var count = faker.Random.Number(1, 4);
            var selectedTagIds = faker.PickRandom(tagIds, count).Distinct();

            foreach (var tagId in selectedTagIds)
            {
                articleTags.Add(new ArticleTag
                {
                    ArticleId = articleId,
                    TagId = tagId
                });
            }
        }

        return articleTags;
    }

    public static List<Comment> GenerateComments(List<int> articleIds, List<Guid> userIds, int count = 30)
    {
        var commentFaker = new Faker<Comment>()
            .RuleFor(c => c.ArticleId, f => f.PickRandom(articleIds))
            .RuleFor(c => c.UserId, f => f.PickRandom(userIds))
            .RuleFor(c => c.Content, f => f.Lorem.Sentence(12))
            .RuleFor(c => c.Status, f => f.PickRandom<CommentStatus>())
            .RuleFor(c => c.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(c => c.UpdatedAt, (f, c) => c.CreatedAt);

        return commentFaker.Generate(count);
    }

    public static List<ArticleRating> GenerateArticleRatings(List<int> articleIds, List<Guid> userIds, int count = 30)
    {
        var faker = new Faker();
        var existingPairs = new HashSet<(int ArticleId, Guid UserId)>();
        var ratings = new List<ArticleRating>();

        var ratingFaker = new Faker<ArticleRating>()
            .RuleFor(r => r.Rating, f => f.Random.Number(1, 5))
            .RuleFor(r => r.CreatedAt, f => f.Date.Recent(60))
            .RuleFor(r => r.UpdatedAt, (f, r) => r.CreatedAt);

        while (ratings.Count < count && existingPairs.Count < (articleIds.Count * userIds.Count))
        {
            var articleId = faker.PickRandom(articleIds);
            var userId = faker.PickRandom(userIds);

            if (existingPairs.Add((articleId, userId)))
            {
                var rating = ratingFaker.Generate();
                rating.ArticleId = articleId;
                rating.UserId = userId;
                ratings.Add(rating);
            }
        }

        return ratings;
    }

    public static SiteSetting GenerateSiteSetting()
    {
        return new Faker<SiteSetting>()
            .RuleFor(s => s.ContactEmail, f => f.Internet.Email())
            .RuleFor(s => s.ContactPhone, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.ContactAddress, f => f.Address.FullAddress())
            .RuleFor(s => s.UpdatedAt, f => f.Date.Recent(10))
            .Generate();
    }

    public static List<SocialLink> GenerateSocialLinks()
    {
        var platforms = new[]
        {
            ("Facebook", "fa-brands fa-facebook", "https://facebook.com/newshub"),
            ("Twitter", "fa-brands fa-twitter", "https://twitter.com/newshub"),
            ("Instagram", "fa-brands fa-instagram", "https://instagram.com/newshub"),
            ("LinkedIn", "fa-brands fa-linkedin", "https://linkedin.com/company/newshub")
        };

        var order = 1;
        var faker = new Faker();

        return platforms.Select(p => new SocialLink
        {
            Platform = p.Item1,
            IconClass = p.Item2,
            Url = p.Item3,
            DisplayOrder = order++,
            IsActive = true,
            CreatedAt = faker.Date.Past(1),
            UpdatedAt = faker.Date.Recent(10)
        }).ToList();
    }

    public static List<Subscription> GenerateSubscriptions(int count = 15)
    {
        var subscriptionFaker = new Faker<Subscription>()
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.UnsubscribeToken, f => Guid.NewGuid())
            .RuleFor(s => s.IsActive, f => f.Random.Bool(0.85f))
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(1))
            .RuleFor(s => s.UnsubscribedAt, (f, s) => s.IsActive ? null : s.CreatedAt.AddDays(f.Random.Number(1, 30)));

        return subscriptionFaker.Generate(count);
    }

    public static List<ContactMessage> GenerateContactMessages(int count = 10)
    {
        var messageFaker = new Faker<ContactMessage>()
            .RuleFor(m => m.Name, f => f.Name.FullName())
            .RuleFor(m => m.Email, f => f.Internet.Email())
            .RuleFor(m => m.Subject, f => f.Lorem.Sentence(4))
            .RuleFor(m => m.Message, f => f.Lorem.Paragraph())
            .RuleFor(m => m.IsRead, f => f.Random.Bool(0.5f))
            .RuleFor(m => m.CreatedAt, f => f.Date.Past(1))
            .RuleFor(m => m.ReadAt, (f, m) => m.IsRead ? m.CreatedAt.AddHours(f.Random.Number(1, 48)) : default);

        return messageFaker.Generate(count);
    }
}