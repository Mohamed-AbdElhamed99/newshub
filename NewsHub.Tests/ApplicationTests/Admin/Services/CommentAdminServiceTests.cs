using Moq;
using NewsHub.Application.Admin.DTOs.Comments;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Admin.Services;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;

namespace NewsHub.Tests.ApplicationTests.Admin.Services;

public class CommentAdminServiceTests
{
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly CommentAdminService _service;

    public CommentAdminServiceTests()
    {
        _mockCommentRepository = new Mock<ICommentRepository>();
        _service = new CommentAdminService(_mockCommentRepository.Object);
    }

    // ---------------------------------------------------------------
    // GetCommentsAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetCommentsAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
    {
        // Arrange
        var comments = new List<Comment>
        {
            new() { Id = 1, ArticleId = 3, UserId = "user-1", Content = "Nice", Status = CommentStatus.Pending }
        };

        var filter = new CommentFilter { Page = 2, PageSize = 5 };

        _mockCommentRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((comments, 17));

        // Act
        var result = await _service.GetCommentsAsync(filter);

        // Assert
        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetCommentsAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
    {
        // Arrange
        var filter = new CommentFilter { Page = 1, PageSize = 10 };

        _mockCommentRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<Comment>(), 0));

        // Act
        var result = await _service.GetCommentsAsync(filter);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetCommentsAsync_MultipleComments_MapsEachToCorrectDto()
    {
        // Arrange
        var comments = new List<Comment>
        {
            new() { Id = 1, ArticleId = 3, UserId = "user-1", Content = "First", Status = CommentStatus.Pending },
            new() { Id = 2, ArticleId = 3, UserId = "user-2", Content = "Second", Status = CommentStatus.Approved }
        };

        var filter = new CommentFilter { Page = 1, PageSize = 10 };

        _mockCommentRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((comments, 2));

        // Act
        var result = await _service.GetCommentsAsync(filter);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Id == 1 && i.Content == "First" && i.Status == CommentStatus.Pending);
        Assert.Contains(result.Items, i => i.Id == 2 && i.Content == "Second" && i.Status == CommentStatus.Approved);
    }

    [Fact]
    public async Task GetCommentsAsync_PassesFilterThroughToRepository()
    {
        // Arrange
        var filter = new CommentFilter { Page = 1, PageSize = 10, ArticleId = 5, Status = CommentStatus.Pending };

        _mockCommentRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<Comment>(), 0));

        // Act
        await _service.GetCommentsAsync(filter);

        // Assert
        _mockCommentRepository.Verify(r => r.GetAllAsync(filter), Times.Once);
    }

    // ---------------------------------------------------------------
    // ApproveCommentAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task ApproveCommentAsync_CommentExists_SetsStatusToApproved()
    {
        // Arrange
        var comment = new Comment { Id = 1, Status = CommentStatus.Pending };
        _mockCommentRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);

        // Act
        var result = await _service.ApproveCommentAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommentStatus.Approved, result!.Status);
        Assert.Equal(CommentStatus.Approved, comment.Status);
    }

    [Fact]
    public async Task ApproveCommentAsync_CommentExists_CallsRepositoryUpdateAsyncOnce()
    {
        // Arrange
        var comment = new Comment { Id = 1, Status = CommentStatus.Pending };
        _mockCommentRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);

        // Act
        await _service.ApproveCommentAsync(1);

        // Assert
        _mockCommentRepository.Verify(r => r.UpdateAsync(comment), Times.Once);
    }

    [Fact]
    public async Task ApproveCommentAsync_CommentNotFound_ReturnsNull()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);

        // Act
        var result = await _service.ApproveCommentAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ApproveCommentAsync_CommentNotFound_NeverCallsUpdateAsync()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);

        // Act
        await _service.ApproveCommentAsync(999);

        // Assert
        _mockCommentRepository.Verify(r => r.UpdateAsync(It.IsAny<Comment>()), Times.Never);
    }

    // ---------------------------------------------------------------
    // RejectCommentAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task RejectCommentAsync_CommentExists_SetsStatusToRejected()
    {
        // Arrange
        var comment = new Comment { Id = 1, Status = CommentStatus.Pending };
        _mockCommentRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);

        // Act
        var result = await _service.RejectCommentAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(CommentStatus.Rejected, result!.Status);
        Assert.Equal(CommentStatus.Rejected, comment.Status);
    }

    [Fact]
    public async Task RejectCommentAsync_CommentExists_CallsRepositoryUpdateAsyncOnce()
    {
        // Arrange
        var comment = new Comment { Id = 1, Status = CommentStatus.Approved };
        _mockCommentRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);

        // Act
        await _service.RejectCommentAsync(1);

        // Assert
        _mockCommentRepository.Verify(r => r.UpdateAsync(comment), Times.Once);
    }

    [Fact]
    public async Task RejectCommentAsync_CommentNotFound_ReturnsNull()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);

        // Act
        var result = await _service.RejectCommentAsync(999);

        // Assert
        Assert.Null(result);
    }

    // ---------------------------------------------------------------
    // DeleteCommentAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task DeleteCommentAsync_CommentExists_ReturnsTrueAndCallsDeleteAsync()
    {
        // Arrange
        var comment = new Comment { Id = 1 };
        _mockCommentRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(comment);

        // Act
        var result = await _service.DeleteCommentAsync(1);

        // Assert
        Assert.True(result);
        _mockCommentRepository.Verify(r => r.DeleteAsync(comment), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentAsync_CommentNotFound_ReturnsFalse()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment?)null);

        // Act
        var result = await _service.DeleteCommentAsync(999);

        // Assert
        Assert.False(result);
        _mockCommentRepository.Verify(r => r.DeleteAsync(It.IsAny<Comment>()), Times.Never);
    }
}