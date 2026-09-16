namespace NewsHub.Tests.ApplicationTests.Site.Services;

using Moq;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

public class SubscriptionServiceTests
{
    private readonly Mock<ISubscriptionRepository> _mockRepository;
    private readonly SubscriptionService _service;

    public SubscriptionServiceTests()
    {
        _mockRepository = new Mock<ISubscriptionRepository>();
        _service = new SubscriptionService(_mockRepository.Object);
    }

    // ---------------------------------------------------------------
    // SubscribeAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task SubscribeAsync_NewEmail_CreatesSubscriptionAndReturnsSuccess()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync((Subscription?)null);

        // Act
        var result = await _service.SubscribeAsync("test@example.com");

        // Assert
        Assert.True(result.Success);
        _mockRepository.Verify(r => r.AddAsync(It.Is<Subscription>(s => s.Email == "test@example.com" && s.IsActive)), Times.Once);
    }

    [Fact]
    public async Task SubscribeAsync_AlreadyActiveSubscription_ReturnsSuccessWithoutDuplicating()
    {
        // Arrange
        var existing = new Subscription { Email = "test@example.com", IsActive = true };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(existing);

        // Act
        var result = await _service.SubscribeAsync("test@example.com");

        // Assert
        Assert.True(result.Success);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Subscription>()), Times.Never);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Subscription>()), Times.Never);
    }

    [Fact]
    public async Task SubscribeAsync_PreviouslyUnsubscribedEmail_ReactivatesExistingSubscription()
    {
        // Arrange
        var existing = new Subscription
        {
            Email = "test@example.com",
            IsActive = false,
            UnsubscribedAt = new DateTime(2026, 1, 1)
        };
        _mockRepository.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(existing);

        // Act
        var result = await _service.SubscribeAsync("test@example.com");

        // Assert
        Assert.True(result.Success);
        Assert.True(existing.IsActive);
        Assert.Null(existing.UnsubscribedAt);
        _mockRepository.Verify(r => r.UpdateAsync(existing), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Subscription>()), Times.Never);
    }

    [Fact]
    public async Task SubscribeAsync_NewEmail_CallsRepositoryGetByEmailAsyncWithCorrectEmail()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((Subscription?)null);

        // Act
        await _service.SubscribeAsync("test@example.com");

        // Assert
        _mockRepository.Verify(r => r.GetByEmailAsync("test@example.com"), Times.Once);
    }

    // ---------------------------------------------------------------
    // UnsubscribeAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task UnsubscribeAsync_ValidTokenActiveSubscription_DeactivatesAndReturnsSuccess()
    {
        // Arrange
        var token = Guid.NewGuid();
        var subscription = new Subscription { UnsubscribeToken = token, IsActive = true };
        _mockRepository.Setup(r => r.GetByTokenAsync(token)).ReturnsAsync(subscription);

        // Act
        var result = await _service.UnsubscribeAsync(token);

        // Assert
        Assert.True(result.Success);
        Assert.False(subscription.IsActive);
        Assert.NotNull(subscription.UnsubscribedAt);
        _mockRepository.Verify(r => r.UpdateAsync(subscription), Times.Once);
    }

    [Fact]
    public async Task UnsubscribeAsync_InvalidToken_ReturnsFailureWithErrorMessage()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByTokenAsync(It.IsAny<Guid>())).ReturnsAsync((Subscription?)null);

        // Act
        var result = await _service.UnsubscribeAsync(Guid.NewGuid());

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Subscription>()), Times.Never);
    }

    [Fact]
    public async Task UnsubscribeAsync_AlreadyInactiveSubscription_ReturnsSuccessIdempotently()
    {
        // Arrange
        var token = Guid.NewGuid();
        var subscription = new Subscription { UnsubscribeToken = token, IsActive = false, UnsubscribedAt = new DateTime(2026, 1, 1) };
        _mockRepository.Setup(r => r.GetByTokenAsync(token)).ReturnsAsync(subscription);

        // Act
        var result = await _service.UnsubscribeAsync(token);

        // Assert
        Assert.True(result.Success);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Subscription>()), Times.Never);
    }

    [Fact]
    public async Task UnsubscribeAsync_ValidToken_CallsRepositoryGetByTokenAsyncWithSameToken()
    {
        // Arrange
        var token = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByTokenAsync(It.IsAny<Guid>())).ReturnsAsync((Subscription?)null);

        // Act
        await _service.UnsubscribeAsync(token);

        // Assert
        _mockRepository.Verify(r => r.GetByTokenAsync(token), Times.Once);
    }
}