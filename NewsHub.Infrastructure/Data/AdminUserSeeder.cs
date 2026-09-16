using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsHub.Infrastructure.Identity;

namespace NewsHub.Infrastructure.Data;

public static class AdminUserSeeder
{
    private static readonly string[] Roles = { "Admin", "User" };

    public static async Task SeedAdminUserAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        var roleManager = provider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = provider.GetRequiredService<IConfiguration>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        var email = configuration["AdminUser:Email"];
        var password = configuration["AdminUser:Password"];
        var userName = configuration["AdminUser:UserName"] ?? "admin";
        var fullName = configuration["AdminUser:FullName"] ?? "Site Administrator";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            // No admin credentials configured (e.g. fresh clone without user
            // secrets set up yet) — skip seeding rather than throwing, so a
            // missing config doesn't crash the whole app on startup.
            return;
        }

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            if (!await userManager.IsInRoleAsync(existing, "Admin"))
            {
                await userManager.AddToRoleAsync(existing, "Admin");
            }

            return;
        }

        
        var admin = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(admin, "Admin");
    }
}