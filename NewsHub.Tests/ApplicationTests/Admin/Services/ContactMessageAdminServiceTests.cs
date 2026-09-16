using Moq;
using NewsHub.Application.Admin.DTOs.ContactMessages;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Admin.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.ApplicationTests.Admin.Services;

public class ContactMessageAdminServiceTests
{
    private readonly Mock<IContactMessageRepository> _mockContactMessageRepository;
    private readonly ContactMessageAdminService _service;

    public ContactMessageAdminServiceTests()
    {
        _mockContactMessageRepository = new Mock<IContactMessageRepository>();
        _service = new ContactMessageAdminService(_mockContactMessageRepository.Object);
    }

    // ---------------------------------------------------------------
    // GetMessagesAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetMessagesAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
    {
        // Arrange
        var messages = new List<ContactMessage>
        {
            new() { Id = 1, Name = "John", Email = "john@test.com", Subject = "Hi", Message = "Hello there" }
        };

        var filter = new ContactMessageFilter { Page = 2, PageSize = 5 };

        _mockContactMessageRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((messages, 17));

        // Act
        var result = await _service.GetMessagesAsync(filter);

        // Assert
        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetMessagesAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
    {
        // Arrange
        var filter = new ContactMessageFilter { Page = 1, PageSize = 10 };

        _mockContactMessageRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<ContactMessage>(), 0));

        // Act
        var result = await _service.GetMessagesAsync(filter);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetMessagesAsync_MultipleMessages_MapsEachToCorrectDto()
    {
        // Arrange
        var messages = new List<ContactMessage>
        {
            new() { Id = 1, Name = "John", Email = "john@test.com", Subject = "Hi", Message = "Hello", IsRead = false },
            new() { Id = 2, Name = "Jane", Email = "jane@test.com", Subject = "Bug", Message = "Found a bug", IsRead = true }
        };

        var filter = new ContactMessageFilter { Page = 1, PageSize = 10 };

        _mockContactMessageRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((messages, 2));

        // Act
        var result = await _service.GetMessagesAsync(filter);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Id == 1 && i.Name == "John" && !i.IsRead);
        Assert.Contains(result.Items, i => i.Id == 2 && i.Name == "Jane" && i.IsRead);
    }

    [Fact]
    public async Task GetMessagesAsync_PassesFilterThroughToRepository()
    {
        // Arrange
        var filter = new ContactMessageFilter { Page = 1, PageSize = 10, IsRead = false };

        _mockContactMessageRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<ContactMessage>(), 0));

        // Act
        await _service.GetMessagesAsync(filter);

        // Assert
        _mockContactMessageRepository.Verify(r => r.GetAllAsync(filter), Times.Once);
    }

    // ---------------------------------------------------------------
    // MarkAsReadAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task MarkAsReadAsync_MessageExists_SetsIsReadTrue()
    {
        // Arrange
        var message = new ContactMessage { Id = 1, IsRead = false };
        _mockContactMessageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(message);

        // Act
        var result = await _service.MarkAsReadAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.True(result!.IsRead);
        Assert.True(message.IsRead);
    }

    [Fact]
    public async Task MarkAsReadAsync_MessageExists_SetsReadAtToUtcNow()
    {
        // Arrange
        var message = new ContactMessage { Id = 1, IsRead = false, ReadAt = default };
        _mockContactMessageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(message);

        var before = DateTime.UtcNow;

        // Act
        var result = await _service.MarkAsReadAsync(1);

        var after = DateTime.UtcNow;

        // Assert
        Assert.True(result!.ReadAt >= before && result.ReadAt <= after);
    }

    [Fact]
    public async Task MarkAsReadAsync_MessageExists_CallsRepositoryUpdateAsyncOnce()
    {
        // Arrange
        var message = new ContactMessage { Id = 1, IsRead = false };
        _mockContactMessageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(message);

        // Act
        await _service.MarkAsReadAsync(1);

        // Assert
        _mockContactMessageRepository.Verify(r => r.UpdateAsync(message), Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_MessageNotFound_ReturnsNull()
    {
        // Arrange
        _mockContactMessageRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ContactMessage?)null);

        // Act
        var result = await _service.MarkAsReadAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task MarkAsReadAsync_MessageNotFound_NeverCallsUpdateAsync()
    {
        // Arrange
        _mockContactMessageRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ContactMessage?)null);

        // Act
        await _service.MarkAsReadAsync(999);

        // Assert
        _mockContactMessageRepository.Verify(r => r.UpdateAsync(It.IsAny<ContactMessage>()), Times.Never);
    }

    // ---------------------------------------------------------------
    // DeleteMessageAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task DeleteMessageAsync_MessageExists_ReturnsTrueAndCallsDeleteAsync()
    {
        // Arrange
        var message = new ContactMessage { Id = 1 };
        _mockContactMessageRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(message);

        // Act
        var result = await _service.DeleteMessageAsync(1);

        // Assert
        Assert.True(result);
        _mockContactMessageRepository.Verify(r => r.DeleteAsync(message), Times.Once);
    }

    [Fact]
    public async Task DeleteMessageAsync_MessageNotFound_ReturnsFalse()
    {
        // Arrange
        _mockContactMessageRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ContactMessage?)null);

        // Act
        var result = await _service.DeleteMessageAsync(999);

        // Assert
        Assert.False(result);
        _mockContactMessageRepository.Verify(r => r.DeleteAsync(It.IsAny<ContactMessage>()), Times.Never);
    }
}