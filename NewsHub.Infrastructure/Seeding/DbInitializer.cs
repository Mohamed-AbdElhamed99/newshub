using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;
using Bogus;
using NewsHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
namespace NewsHub.Infrastructure.Seeding;

public static class DbInitializer
{
    public static async Task SeedAsync(NewsHubDbContext context, UserManager<ApplicationUser> userManager)
    {
        // 1. Ensure DB is created / Migrations applied
        await context.Database.EnsureCreatedAsync();

        // 2. Prevent duplicate seeding
        if (await context.Set<Article>().AnyAsync())
            return;

        var mockUsers = new List<ApplicationUser>();
        for (int i = 1; i <= 5; i++)
        {
            var userEmail = $"user{i}@newshub.com";
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Password123!");
            }
            mockUsers.Add(user);
        }
        
        // Generate static User IDs to simulate logged-in authors and readers
        var authorIds = mockUsers.Take(2).Select(u => u.Id).ToList();
        var readerIds = mockUsers.Skip(2).Select(u => u.Id).ToList();

        // 3. Seed Base Entities (No Dependencies)
        var categories = DataFactories.GenerateCategories(count: 6);
        var tags = DataFactories.GenerateTags(count: 12);
        var siteSetting = DataFactories.GenerateSiteSetting();
        var socialLinks = DataFactories.GenerateSocialLinks();
        var subscriptions = DataFactories.GenerateSubscriptions(count: 20);
        var contactMessages = DataFactories.GenerateContactMessages(count: 15);

        await context.Set<Category>().AddRangeAsync(categories);
        await context.Set<Tag>().AddRangeAsync(tags);
        await context.Set<SiteSetting>().AddAsync(siteSetting);
        await context.Set<SocialLink>().AddRangeAsync(socialLinks);
        await context.Set<Subscription>().AddRangeAsync(subscriptions);
        await context.Set<ContactMessage>().AddRangeAsync(contactMessages);

        await context.SaveChangesAsync();

        // 4. Seed Articles & Translations
        var articles = DataFactories.GenerateArticles(categories, authorIds, count: 25);
        await context.Set<Article>().AddRangeAsync(articles);
        await context.SaveChangesAsync();

        // Extract Auto-Generated IDs
        var articleIds = articles.Select(a => a.Id).ToList();
        var tagIds = tags.Select(t => t.Id).ToList();

        // 5. Seed Dependent Relational Entities
        var articleTags = DataFactories.GenerateArticleTags(articleIds, tagIds);
        var comments = DataFactories.GenerateComments(articleIds, readerIds, count: 50);
        var ratings = DataFactories.GenerateArticleRatings(articleIds, readerIds, count: 40);

        await context.Set<ArticleTag>().AddRangeAsync(articleTags);
        await context.Set<Comment>().AddRangeAsync(comments);
        await context.Set<ArticleRating>().AddRangeAsync(ratings);

        await context.SaveChangesAsync();
    }
}