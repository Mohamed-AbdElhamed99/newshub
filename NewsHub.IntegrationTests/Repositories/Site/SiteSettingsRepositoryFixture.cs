using NewsHub.Domain.Entities;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class SiteSettingsRepositoryFixture : SqliteInMemoryFixtureBase
{
    public SiteSetting SeedSettings(string email, string phone, string address)
    {
        var settings = new SiteSetting
        {
            ContactEmail = email,
            ContactPhone = phone,
            ContactAddress = address
        };
        Context.SiteSettings.Add(settings);
        Context.SaveChanges();
        return settings;
    }

    public SocialLink SeedSocialLink(string platform, string url, int displayOrder, bool isActive = true)
    {
        var link = new SocialLink
        {
            Platform = platform,
            Url = url,
            DisplayOrder = displayOrder,
            IsActive = isActive
        };
        Context.SocialLinks.Add(link);
        Context.SaveChanges();
        return link;
    }
}
