using NewsHub.Domain.Entities;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class SubscriptionRepositoryFixture : SqliteInMemoryFixtureBase
{
    public Subscription SeedSubscription(string? email = null, bool isActive = true)
    {
        var subscription = new Subscription
        {
            Email = email ?? $"{Guid.NewGuid():N}@test.com",
            IsActive = isActive
        };
        Context.Subscriptions.Add(subscription);
        Context.SaveChanges();
        return subscription;
    }
}
