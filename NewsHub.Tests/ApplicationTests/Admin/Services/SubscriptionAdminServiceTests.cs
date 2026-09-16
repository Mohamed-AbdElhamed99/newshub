using Moq;
using NewsHub.Application.Admin.DTOs.Subscriptions;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Admin.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.ApplicationTests.Admin.Services;

public class SubscriptionAdminServiceTests
{
    private readonly Mock<ISubscriptionRepository> _mockSubscriptionRepository;
    private readonly SubscriptionAdminService _service;

    public SubscriptionAdminServiceTests()
    {
        _mockSubscriptionRepository = new Mock<ISubscriptionRepository>();
        _service = new SubscriptionAdminService(_mockSubscriptionRepository.Object);
    }

    // ---------------------------------------------------------------
    // GetSubscriptionsAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetSubscriptionsAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new() { Id = 1, Email = "test@test.com", IsActive = true }
        };

        var filter = new SubscriptionFilter { Page = 2, PageSize = 5 };

        _mockSubscriptionRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((subscriptions, 17));

        // Act
        var result = await _service.GetSubscriptionsAsync(filter);

        // Assert
        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetSubscriptionsAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
    {
        // Arrange
        var filter = new SubscriptionFilter { Page = 1, PageSize = 10 };

        _mockSubscriptionRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<Subscription>(), 0));

        // Act
        var result = await _service.GetSubscriptionsAsync(filter);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetSubscriptionsAsync_MultipleSubscriptions_MapsEachToCorrectDto()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new() { Id = 1, Email = "john@test.com", IsActive = true },
            new() { Id = 2, Email = "jane@test.com", IsActive = false, UnsubscribedAt = DateTime.UtcNow }
        };

        var filter = new SubscriptionFilter { Page = 1, PageSize = 10 };

        _mockSubscriptionRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((subscriptions, 2));

        // Act
        var result = await _service.GetSubscriptionsAsync(filter);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Id == 1 && i.Email == "john@test.com" && i.IsActive);
        Assert.Contains(result.Items, i => i.Id == 2 && i.Email == "jane@test.com" && !i.IsActive);
    }

    [Fact]
    public async Task GetSubscriptionsAsync_PassesFilterThroughToRepository()
    {
        // Arrange
        var filter = new SubscriptionFilter { Page = 1, PageSize = 10, IsActive = true };

        _mockSubscriptionRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<Subscription>(), 0));

        // Act
        await _service.GetSubscriptionsAsync(filter);

        // Assert
        _mockSubscriptionRepository.Verify(r => r.GetAllAsync(filter), Times.Once);
    }

    // ---------------------------------------------------------------
    // DeactivateSubscriptionAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task DeactivateSubscriptionAsync_SubscriptionExists_SetsIsActiveFalse()
    {
        // Arrange
        var subscription = new Subscription { Id = 1, IsActive = true };
        _mockSubscriptionRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);

        // Act
        var result = await _service.DeactivateSubscriptionAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.False(result!.IsActive);
        Assert.False(subscription.IsActive);
    }

    [Fact]
    public async Task DeactivateSubscriptionAsync_SubscriptionExists_SetsUnsubscribedAtToUtcNow()
    {
        // Arrange
        var subscription = new Subscription { Id = 1, IsActive = true, UnsubscribedAt = null };
        _mockSubscriptionRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);

        var before = DateTime.UtcNow;

        // Act
        var result = await _service.DeactivateSubscriptionAsync(1);

        var after = DateTime.UtcNow;

        // Assert
        Assert.NotNull(result!.UnsubscribedAt);
        Assert.True(result.UnsubscribedAt >= before && result.UnsubscribedAt <= after);
    }

    [Fact]
    public async Task DeactivateSubscriptionAsync_SubscriptionExists_CallsRepositoryUpdateAsyncOnce()
    {
        // Arrange
        var subscription = new Subscription { Id = 1, IsActive = true };
        _mockSubscriptionRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);

        // Act
        await _service.DeactivateSubscriptionAsync(1);

        // Assert
        _mockSubscriptionRepository.Verify(r => r.UpdateAsync(subscription), Times.Once);
    }

    [Fact]
    public async Task DeactivateSubscriptionAsync_SubscriptionNotFound_ReturnsNull()
    {
        // Arrange
        _mockSubscriptionRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Subscription?)null);

        // Act
        var result = await _service.DeactivateSubscriptionAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeactivateSubscriptionAsync_SubscriptionNotFound_NeverCallsUpdateAsync()
    {
        // Arrange
        _mockSubscriptionRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Subscription?)null);

        // Act
        await _service.DeactivateSubscriptionAsync(999);

        // Assert
        _mockSubscriptionRepository.Verify(r => r.UpdateAsync(It.IsAny<Subscription>()), Times.Never);
    }

    // ---------------------------------------------------------------
    // DeleteSubscriptionAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task DeleteSubscriptionAsync_SubscriptionExists_ReturnsTrueAndCallsDeleteAsync()
    {
        // Arrange
        var subscription = new Subscription { Id = 1 };
        _mockSubscriptionRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(subscription);

        // Act
        var result = await _service.DeleteSubscriptionAsync(1);

        // Assert
        Assert.True(result);
        _mockSubscriptionRepository.Verify(r => r.DeleteAsync(subscription), Times.Once);
    }

    [Fact]
    public async Task DeleteSubscriptionAsync_SubscriptionNotFound_ReturnsFalse()
    {
        // Arrange
        _mockSubscriptionRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Subscription?)null);

        // Act
        var result = await _service.DeleteSubscriptionAsync(999);

        // Assert
        Assert.False(result);
        _mockSubscriptionRepository.Verify(r => r.DeleteAsync(It.IsAny<Subscription>()), Times.Never);
    }
}