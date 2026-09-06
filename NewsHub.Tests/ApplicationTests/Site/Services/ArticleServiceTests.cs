namespace NewsHub.Tests.ApplicationTests.Site.Services;

using System.Globalization;
using Moq;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

public class ArticleServiceTests
{
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly ArticleService _service;

    public ArticleServiceTests()
    {
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockCommentRepository = new Mock<ICommentRepository>();
        _service = new ArticleService(_mockArticleRepository.Object, _mockCommentRepository.Object);
    }

    // ---------------------------------------------------------------
    // GetLatestAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetLatestAsync_RepositoryReturnsArticles_MapsToLatestDtoCorrectly()
    {
        // Arrange
        var articles = new List<Article>
        {
            new()
            {
                Id = 1,
                ImageUrl = "/images/tech.png",
                ViewCount = 42,
                PublishedAt = new DateTime(2026, 1, 1),
                Translations = new List<ArticleTranslation>
                {
                    new() { LanguageCode = "en", Title = "Tech News", Slug = "tech-news", Excerpt = "Short excerpt", Content = "Body" }
                }
            }
        };

        _mockArticleRepository.Setup(r => r.GetLatestAsync(10)).ReturnsAsync(articles);
        _mockCommentRepository
            .Setup(r => r.GetApprovedCommentCountsAsync(It.Is<IEnumerable<int>>(ids => ids.Contains(1))))
            .ReturnsAsync(new Dictionary<int, int> { [1] = 5 });

        // Act
        var result = await _service.GetLatestAsync(10);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Tech News", dto.Title);
        Assert.Equal(42, dto.ViewCount);
        Assert.Equal(5, dto.CommentCount);
        Assert.Equal(new DateTime(2026, 1, 1), dto.PublishedAt);
    }

    [Fact]
    public async Task GetLatestAsync_ArticleHasNoCommentCountEntry_DefaultsToZero()
    {
        // Arrange
        var articles = new List<Article>
        {
            new()
            {
                Id = 1,
                Translations = new List<ArticleTranslation>
                {
                    new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
                }
            }
        };

        _mockArticleRepository.Setup(r => r.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(articles);
        _mockCommentRepository
            .Setup(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, int>()); // no entry for article 1

        // Act
        var result = await _service.GetLatestAsync(10);

        // Assert
        Assert.Equal(0, result.First().CommentCount);
    }

    [Fact]
    public async Task GetLatestAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<Article>());
        _mockCommentRepository.Setup(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, int>());

        // Act
        await _service.GetLatestAsync(10);

        // Assert
        _mockArticleRepository.Verify(r => r.GetLatestAsync(10), Times.Once);
    }

    [Fact]
    public async Task GetLatestAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<Article>());
        _mockCommentRepository.Setup(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, int>());

        // Act
        var result = await _service.GetLatestAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLatestAsync_EmptyArticleList_DoesNotCallCommentRepositoryWithNullIds()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<Article>());
        _mockCommentRepository.Setup(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, int>());

        // Act
        await _service.GetLatestAsync(10);

        // Assert
        _mockCommentRepository.Verify(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()), Times.Once);
    }

    // ---------------------------------------------------------------
    // GetTopStoryAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetTopStoryAsync_RepositoryReturnsArticle_MapsToTopStoryDtoCorrectly()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            ImageUrl = "/images/top.png",
            ViewCount = 999,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Biggest Story", Slug = "biggest-story", Excerpt = "Excerpt", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetTopStoryAsync()).ReturnsAsync(article);

        // Act
        var result = await _service.GetTopStoryAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Biggest Story", result.Title);
        Assert.Equal(999, result.ViewCount);
    }

    [Fact]
    public async Task GetTopStoryAsync_RepositoryReturnsNull_ReturnsNull()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetTopStoryAsync()).ReturnsAsync((Article?)null);

        // Act
        var result = await _service.GetTopStoryAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTopStoryAsync_CallsRepositoryGetTopStoryAsyncOnce()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetTopStoryAsync()).ReturnsAsync((Article?)null);

        // Act
        await _service.GetTopStoryAsync();

        // Assert
        _mockArticleRepository.Verify(r => r.GetTopStoryAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTopStoryAsync_NoCultureMatch_FallsBackToEnglish()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var article = new Article
            {
                Id = 1,
                Translations = new List<ArticleTranslation>
                {
                    new() { LanguageCode = "en", Title = "Top", Slug = "top", Content = "Body" },
                    new() { LanguageCode = "ar", Title = "الأهم", Slug = "top-ar", Content = "محتوى" }
                }
            };

            _mockArticleRepository.Setup(r => r.GetTopStoryAsync()).ReturnsAsync(article);

            // Act
            var result = await _service.GetTopStoryAsync();

            // Assert
            Assert.Equal("Top", result!.Title);
            Assert.Equal("en", result.LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    // ---------------------------------------------------------------
    // GetMostViewedAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetMostViewedAsync_RepositoryReturnsArticles_MapsToLatestDtoCorrectly()
    {
        // Arrange
        var articles = new List<Article>
        {
            new()
            {
                Id = 1,
                ImageUrl = "/images/viewed.png",
                ViewCount = 500,
                Translations = new List<ArticleTranslation>
                {
                    new() { LanguageCode = "en", Title = "Most Viewed", Slug = "most-viewed", Content = "Body" }
                }
            }
        };

        _mockArticleRepository.Setup(r => r.GetMostViewedAsync(5)).ReturnsAsync(articles);
        _mockCommentRepository
            .Setup(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, int> { [1] = 3 });

        // Act
        var result = await _service.GetMostViewedAsync(5);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(1, dto.Id);
        Assert.Equal(500, dto.ViewCount);
        Assert.Equal(3, dto.CommentCount);
    }

    [Fact]
    public async Task GetMostViewedAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetMostViewedAsync(It.IsAny<int>())).ReturnsAsync(new List<Article>());
        _mockCommentRepository.Setup(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, int>());

        // Act
        await _service.GetMostViewedAsync(5);

        // Assert
        _mockArticleRepository.Verify(r => r.GetMostViewedAsync(5), Times.Once);
    }

    [Fact]
    public async Task GetMostViewedAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetMostViewedAsync(It.IsAny<int>())).ReturnsAsync(new List<Article>());
        _mockCommentRepository.Setup(r => r.GetApprovedCommentCountsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync(new Dictionary<int, int>());

        // Act
        var result = await _service.GetMostViewedAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}