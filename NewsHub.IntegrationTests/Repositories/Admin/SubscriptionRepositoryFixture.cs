using NewsHub.Domain.Entities;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class SubscriptionRepositoryFixture : SqliteInMemoryFixtureBase
{
    public Subscription SeedSubscription(bool isActive = true, string? email = null)
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