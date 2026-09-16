using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Services;
using NewsHub.Application.Common.Random;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Application.Site.Services;

namespace NewsHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ICategoryAdminService, CategoryAdminService>();
        services.AddScoped<ITagAdminService, TagAdminService>();
        services.AddScoped<IArticleAdminService, ArticleAdminService>();
        services.AddScoped<IArticleRatingAdminService, ArticleRatingAdminService>();
        services.AddScoped<ICommentAdminService, CommentAdminService>();
        services.AddScoped<ISubscriptionAdminService, SubscriptionAdminService>();
        services.AddScoped<IContactMessageAdminService, ContactMessageAdminService>();
        services.AddScoped<ISiteSettingAdminService, SiteSettingAdminService>();
        services.AddScoped<ISocialLinkAdminService, SocialLinkAdminService >();
        
        services.AddScoped<IArticleService, ArticleService >();
        services.AddScoped<IArticleInteractionService, ArticleInteractionService >();
        services.AddScoped<ICategoryService, CategoryService >();
        services.AddScoped<IContactMessageService, ContactMessageService >();
        services.AddScoped<IHomePageService, HomePageService >();
        services.AddScoped<ILayoutService, LayoutService >();
        services.AddScoped<ISubscriptionService, SubscriptionService >();
        services.AddScoped<ITagService, TagService >();
        
        
        services.AddScoped<IRandomProvider, RandomProvider >();
        return services;
    }
}