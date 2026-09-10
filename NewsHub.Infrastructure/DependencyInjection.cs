using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Infrastructure.Data;
using NewsHub.Infrastructure.Identity;
using NewsHub.Infrastructure.Services;

// Aliases to disambiguate Admin vs Site repository interfaces/implementations
// that share the same short name across namespaces.
using AdminRepos = NewsHub.Application.Admin.Interfaces.Repositories;
using SiteRepos = NewsHub.Application.Site.Interfaces.Repositories;
using AdminImpl = NewsHub.Infrastructure.Repositories.Admin;
using SiteImpl = NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // --- DbContext ---
        services.AddDbContext<NewsHubDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // --- Identity ---
        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<NewsHubDbContext>()
            .AddDefaultTokenProviders();

        // --- Admin repositories ---
        services.AddScoped<AdminRepos.IArticleRepository, AdminImpl.ArticleRepository>();
        services.AddScoped<AdminRepos.IArticleRatingRepository, AdminImpl.ArticleRatingRepository>();
        services.AddScoped<AdminRepos.ICategoryRepository, AdminImpl.CategoryRepository>();
        services.AddScoped<AdminRepos.ICommentRepository, AdminImpl.CommentRepository>();
        services.AddScoped<AdminRepos.IContactMessageRepository, AdminImpl.ContactMessageRepository>();
        services.AddScoped<AdminRepos.ISubscriptionRepository, AdminImpl.SubscriptionRepository>();
        services.AddScoped<AdminRepos.ITagRepository, AdminImpl.TagRepository>();

        // --- Site repositories ---
        services.AddScoped<SiteRepos.IArticleRepository, SiteImpl.ArticleRepository>();
        services.AddScoped<SiteRepos.IArticleRatingRepository, SiteImpl.ArticleRatingRepository>();
        services.AddScoped<SiteRepos.ICategoryRepository, SiteImpl.CategoryRepository>();
        services.AddScoped<SiteRepos.ICommentRepository, SiteImpl.CommentRepository>();
        services.AddScoped<SiteRepos.IContactMessageRepository, SiteImpl.ContactMessageRepository>();
        services.AddScoped<SiteRepos.ISiteSettingsRepository, SiteImpl.SiteSettingsRepository>();
        services.AddScoped<SiteRepos.ISubscriptionRepository, SiteImpl.SubscriptionRepository>();
        services.AddScoped<SiteRepos.ITagRepository, SiteImpl.TagRepository>();

        // --- Services ---
        services.AddScoped<IAuthService, AuthService>();

        // TODO: confirm your concrete IEmailSender implementation's class
        // name/namespace (e.g. SmtpEmailSender) — not shown to me yet.
        // services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}