using NewsHub.Infrastructure.Data;

namespace NewsHub.IntegrationTests.Auth;

using NewsHub.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Infrastructure.Identity;

public class AuthServiceFixture : IDisposable
{
    public ServiceProvider Provider { get; }
    public Mock<IEmailSender> EmailSenderMock { get; } = new();

    public AuthServiceFixture()
    {
        var services = new ServiceCollection();
        var dbName = $"NewsHubTests_{Guid.NewGuid()}";

        services.AddDbContext<NewsHubDbContext>(options =>
            options.UseInMemoryDatabase(dbName));

        services.AddHttpContextAccessor();
        services.AddAuthentication();

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<NewsHubDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton(EmailSenderMock.Object);
        services.AddScoped<IAuthService, AuthService>();
        services.AddLogging();

        Provider = services.BuildServiceProvider();

        SeedRoles("User", "Admin");
    }

    private void SeedRoles(params string[] roleNames)
    {
        using var scope = Provider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        foreach (var roleName in roleNames)
        {
            var result = roleManager.CreateAsync(new IdentityRole<Guid>(roleName)).GetAwaiter().GetResult();
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to seed role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public IAuthService CreateAuthService() =>
        Provider.CreateScope().ServiceProvider.GetRequiredService<IAuthService>();

    public void Dispose() => Provider.Dispose();
}