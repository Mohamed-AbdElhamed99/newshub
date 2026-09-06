using Moq;
using NewsHub.Application.Admin.DTOs.ArticleRatings;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Admin.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.ApplicationTests.Admin.Services;

public class ArticleRatingAdminServiceTests
{
    private readonly Mock<IArticleRatingRepository> _mockRatingRepository;
    private readonly ArticleRatingAdminService _service;

    public ArticleRatingAdminServiceTests()
    {
        _mockRatingRepository = new Mock<IArticleRatingRepository>();
        _service = new ArticleRatingAdminService(_mockRatingRepository.Object);
    }

    // ---------------------------------------------------------------
    // GetRatingsAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetRatingsAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
    {
        // Arrange
        var ratings = new List<ArticleRating>
        {
            new() { Id = 1, ArticleId = 3, UserId = Guid.NewGuid(), Rating = 5 }
        };

        var filter = new ArticleRatingFilter { Page = 2, PageSize = 5 };

        _mockRatingRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((ratings, 17));

        // Act
        var result = await _service.GetRatingsAsync(filter);

        // Assert
        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetRatingsAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
    {
        // Arrange
        var filter = new ArticleRatingFilter { Page = 1, PageSize = 10 };

        _mockRatingRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<ArticleRating>(), 0));

        // Act
        var result = await _service.GetRatingsAsync(filter);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetRatingsAsync_MultipleRatings_MapsEachToCorrectDto()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        // Arrange
        var ratings = new List<ArticleRating>
        {
            new() { Id = 1, ArticleId = 3, UserId = userId1, Rating = 5 },
            new() { Id = 2, ArticleId = 3, UserId = userId2, Rating = 2 }
        };

        var filter = new ArticleRatingFilter { Page = 1, PageSize = 10 };

        _mockRatingRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((ratings, 2));

        // Act
        var result = await _service.GetRatingsAsync(filter);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Id == 1 && i.Rating == 5);
        Assert.Contains(result.Items, i => i.Id == 2 && i.Rating == 2);
    }

    [Fact]
    public async Task GetRatingsAsync_PassesFilterThroughToRepository()
    {
        // Arrange
        var filter = new ArticleRatingFilter { Page = 1, PageSize = 10, ArticleId = 5 };

        _mockRatingRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<ArticleRating>(), 0));

        // Act
        await _service.GetRatingsAsync(filter);

        // Assert
        _mockRatingRepository.Verify(r => r.GetAllAsync(filter), Times.Once);
    }

    // ---------------------------------------------------------------
    // GetSummaryByArticleIdAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetSummaryByArticleIdAsync_RatingsExist_ReturnsCorrectAverageAndCount()
    {
        // Arrange
        _mockRatingRepository
            .Setup(r => r.GetSummaryByArticleIdAsync(3))
            .ReturnsAsync((3.5, 10));

        // Act
        var result = await _service.GetSummaryByArticleIdAsync(3);

        // Assert
        Assert.Equal(3, result.ArticleId);
        Assert.Equal(3.5, result.AverageRating);
        Assert.Equal(10, result.TotalRatings);
    }

    [Fact]
    public async Task GetSummaryByArticleIdAsync_NoRatings_ReturnsZeroAverageAndCount()
    {
        // Arrange
        _mockRatingRepository
            .Setup(r => r.GetSummaryByArticleIdAsync(9))
            .ReturnsAsync((0, 0));

        // Act
        var result = await _service.GetSummaryByArticleIdAsync(9);

        // Assert
        Assert.Equal(0, result.AverageRating);
        Assert.Equal(0, result.TotalRatings);
    }

    [Fact]
    public async Task GetSummaryByArticleIdAsync_CallsRepositoryWithCorrectArticleId()
    {
        // Arrange
        _mockRatingRepository
            .Setup(r => r.GetSummaryByArticleIdAsync(7))
            .ReturnsAsync((4.2, 5));

        // Act
        await _service.GetSummaryByArticleIdAsync(7);

        // Assert
        _mockRatingRepository.Verify(r => r.GetSummaryByArticleIdAsync(7), Times.Once);
    }

    // ---------------------------------------------------------------
    // DeleteRatingAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task DeleteRatingAsync_RatingExists_ReturnsTrueAndCallsDeleteAsync()
    {
        // Arrange
        var rating = new ArticleRating { Id = 1 };
        _mockRatingRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(rating);

        // Act
        var result = await _service.DeleteRatingAsync(1);

        // Assert
        Assert.True(result);
        _mockRatingRepository.Verify(r => r.DeleteAsync(rating), Times.Once);
    }

    [Fact]
    public async Task DeleteRatingAsync_RatingNotFound_ReturnsFalse()
    {
        // Arrange
        _mockRatingRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((ArticleRating?)null);

        // Act
        var result = await _service.DeleteRatingAsync(999);

        // Assert
        Assert.False(result);
        _mockRatingRepository.Verify(r => r.DeleteAsync(It.IsAny<ArticleRating>()), Times.Never);
    }
}