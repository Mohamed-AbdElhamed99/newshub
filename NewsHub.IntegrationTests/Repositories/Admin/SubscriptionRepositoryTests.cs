using FluentAssertions;
using NewsHub.Application.Admin.DTOs.Subscriptions;
using NewsHub.Infrastructure.Repositories.Admin;

namespace NewsHub.IntegrationTests.Repositories.Admin;

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
    public async Task GetByIdAsync_WhenSubscriptionExists_ReturnsSubscription()
    {
        var seeded = _fixture.SeedSubscription();

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(seeded.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubscriptionDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllSubscriptionsAndCorrectTotalCount()
    {
        _fixture.SeedSubscription();
        _fixture.SeedSubscription();

        var (items, totalCount) = await _sut.GetAllAsync(new SubscriptionFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByIsActive_ReturnsOnlyMatchingSubscriptions()
    {
        _fixture.SeedSubscription(isActive: true);
        _fixture.SeedSubscription(isActive: false);

        var (items, totalCount) = await _sut.GetAllAsync(new SubscriptionFilter { IsActive = true });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(s => s.IsActive);
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

    [Fact]
    public async Task DeleteAsync_RemovesSubscriptionFromDatabase()
    {
        var seeded = _fixture.SeedSubscription();

        using (var deleteContext = _fixture.CreateAssertionContext())
        {
            var toDelete = await deleteContext.Subscriptions.FindAsync(seeded.Id);
            var repo = new SubscriptionRepository(deleteContext);
            await repo.DeleteAsync(toDelete!);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var deleted = await assertContext.Subscriptions.FindAsync(seeded.Id);

        deleted.Should().BeNull();
    }
}