using FluentAssertions;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class SubscriptionRepositoryTests : IDisposable
{
    private readonly SubscriptionRepositoryFixture _fixture;
    private readonly SubscriptionRepository _sut;

    public SubscriptionRepositoryTests()
    {
        _fixture = new SubscriptionRepositoryFixture();
        _sut = new SubscriptionRepository(_fixture.Context);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task GetByEmailAsync_WhenSubscriptionExists_ReturnsSubscription()
    {
        var seeded = _fixture.SeedSubscription(email: "reader@newshub.com");

        var result = await _sut.GetByEmailAsync("reader@newshub.com");

        result.Should().NotBeNull();
        result!.Id.Should().Be(seeded.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenSubscriptionDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByEmailAsync("nobody@newshub.com");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByTokenAsync_WhenTokenMatches_ReturnsSubscription()
    {
        var seeded = _fixture.SeedSubscription();

        var result = await _sut.GetByTokenAsync(seeded.UnsubscribeToken);

        result.Should().NotBeNull();
        result!.Id.Should().Be(seeded.Id);
    }

    [Fact]
    public async Task GetByTokenAsync_WhenTokenDoesNotMatch_ReturnsNull()
    {
        var result = await _sut.GetByTokenAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ValidSubscription_PersistsToDatabase()
    {
        var subscription = new Subscription { Email = "new@newshub.com" };

        await _sut.AddAsync(subscription);

        using var assertContext = _fixture.CreateAssertionContext();
        var saved = await assertContext.Subscriptions.FindAsync(subscription.Id);

        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingSubscription()
    {
        var seeded = _fixture.SeedSubscription(isActive: true);

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.Subscriptions.FindAsync(seeded.Id);
            toUpdate!.IsActive = false;
            toUpdate.UnsubscribedAt = DateTime.UtcNow;

            var repo = new SubscriptionRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Subscriptions.FindAsync(seeded.Id);

        updated!.IsActive.Should().BeFalse();
    }
}
