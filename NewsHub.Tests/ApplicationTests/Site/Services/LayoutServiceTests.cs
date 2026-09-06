namespace NewsHub.Tests.ApplicationTests.Site.Services;

using System.Globalization;
using Moq;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.DTOs.Layout;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

public class LayoutServiceTests
{
    private readonly Mock<IArticleService> _mockArticleService;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<ISiteSettingsRepository> _mockSiteSettingsRepository;
    private readonly LayoutService _service;

    public LayoutServiceTests()
    {
        _mockArticleService = new Mock<IArticleService>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockSiteSettingsRepository = new Mock<ISiteSettingsRepository>();

        _service = new LayoutService(
            _mockArticleService.Object,
            _mockCategoryRepository.Object,
            _mockSiteSettingsRepository.Object);
    }

    // ---------------------------------------------------------------
    // GetRecentPostsAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetRecentPostsAsync_ArticleServiceReturnsData_ReturnsSameData()
    {
        // Arrange
        var posts = new List<LatestArticleDto>
        {
            new() { Id = 1, Title = "Post One" }
        };

        _mockArticleService.Setup(s => s.GetLatestAsync(5)).ReturnsAsync(posts);

        // Act
        var result = await _service.GetRecentPostsAsync(5);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal("Post One", dto.Title);
    }

    [Fact]
    public async Task GetRecentPostsAsync_ValidCount_CallsArticleServiceWithSameCount()
    {
        // Arrange
        _mockArticleService.Setup(s => s.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<LatestArticleDto>());

        // Act
        await _service.GetRecentPostsAsync(5);

        // Assert
        _mockArticleService.Verify(s => s.GetLatestAsync(5), Times.Once);
    }

    [Fact]
    public async Task GetRecentPostsAsync_ArticleServiceReturnsEmptyList_ReturnsEmptyNotNull()
    {
        // Arrange
        _mockArticleService.Setup(s => s.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<LatestArticleDto>());

        // Act
        var result = await _service.GetRecentPostsAsync(5);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // ---------------------------------------------------------------
    // GetFooterCategoriesAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetFooterCategoriesAsync_RepositoryReturnsCategories_MapsToCategoryDtoCorrectly()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                Id = 1,
                Translations = new List<CategoryTranslation>
                {
                    new() { LanguageCode = "en", Name = "Sports", Slug = "sports" }
                }
            }
        };

        _mockCategoryRepository.Setup(r => r.GetTopNAsync(6)).ReturnsAsync(categories);

        // Act
        var result = await _service.GetFooterCategoriesAsync(6);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Sports", dto.Name);
        Assert.Equal("sports", dto.Slug);
    }

    [Fact]
    public async Task GetFooterCategoriesAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        // Arrange
        _mockCategoryRepository.Setup(r => r.GetTopNAsync(It.IsAny<int>())).ReturnsAsync(new List<Category>());

        // Act
        await _service.GetFooterCategoriesAsync(6);

        // Assert
        _mockCategoryRepository.Verify(r => r.GetTopNAsync(6), Times.Once);
    }

    [Fact]
    public async Task GetFooterCategoriesAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        // Arrange
        _mockCategoryRepository.Setup(r => r.GetTopNAsync(It.IsAny<int>())).ReturnsAsync(new List<Category>());

        // Act
        var result = await _service.GetFooterCategoriesAsync(6);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetFooterCategoriesAsync_NoCultureMatch_FallsBackToEnglish()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var categories = new List<Category>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<CategoryTranslation>
                    {
                        new() { LanguageCode = "en", Name = "Sports", Slug = "sports" },
                        new() { LanguageCode = "ar", Name = "رياضة", Slug = "sports-ar" }
                    }
                }
            };

            _mockCategoryRepository.Setup(r => r.GetTopNAsync(It.IsAny<int>())).ReturnsAsync(categories);

            // Act
            var result = await _service.GetFooterCategoriesAsync(6);

            // Assert
            Assert.Equal("Sports", result.First().Name);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public async Task GetFooterCategoriesAsync_NoCultureOrEnglishMatch_FallsBackToFirstTranslation()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var categories = new List<Category>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<CategoryTranslation>
                    {
                        new() { LanguageCode = "ar", Name = "رياضة", Slug = "sports-ar" },
                        new() { LanguageCode = "de", Name = "Sport", Slug = "sports-de" }
                    }
                }
            };

            _mockCategoryRepository.Setup(r => r.GetTopNAsync(It.IsAny<int>())).ReturnsAsync(categories);

            // Act
            var result = await _service.GetFooterCategoriesAsync(6);

            // Assert
            Assert.Equal("رياضة", result.First().Name);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public async Task GetFooterCategoriesAsync_MultipleCategories_MapsEachCorrectly()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Translations = new List<CategoryTranslation> { new() { LanguageCode = "en", Name = "Sports", Slug = "sports" } } },
            new() { Id = 2, Translations = new List<CategoryTranslation> { new() { LanguageCode = "en", Name = "Tech", Slug = "tech" } } }
        };

        _mockCategoryRepository.Setup(r => r.GetTopNAsync(It.IsAny<int>())).ReturnsAsync(categories);

        // Act
        var result = await _service.GetFooterCategoriesAsync(6);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == 1 && c.Name == "Sports");
        Assert.Contains(result, c => c.Id == 2 && c.Name == "Tech");
    }

    // ---------------------------------------------------------------
    // GetGalleryImageUrlsAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetGalleryImageUrlsAsync_ArticleServiceReturnsPosts_ReturnsTheirImageUrls()
    {
        // Arrange
        var posts = new List<LatestArticleDto>
        {
            new() { Id = 1, ImageUrl = "/images/one.png" },
            new() { Id = 2, ImageUrl = "/images/two.png" }
        };

        _mockArticleService.Setup(s => s.GetLatestAsync(6)).ReturnsAsync(posts);

        // Act
        var result = await _service.GetGalleryImageUrlsAsync(6);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains("/images/one.png", result);
        Assert.Contains("/images/two.png", result);
    }

    [Fact]
    public async Task GetGalleryImageUrlsAsync_ValidCount_CallsArticleServiceWithSameCount()
    {
        // Arrange
        _mockArticleService.Setup(s => s.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<LatestArticleDto>());

        // Act
        await _service.GetGalleryImageUrlsAsync(6);

        // Assert
        _mockArticleService.Verify(s => s.GetLatestAsync(6), Times.Once);
    }

    [Fact]
    public async Task GetGalleryImageUrlsAsync_ArticleServiceReturnsEmptyList_ReturnsEmptyNotNull()
    {
        // Arrange
        _mockArticleService.Setup(s => s.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<LatestArticleDto>());

        // Act
        var result = await _service.GetGalleryImageUrlsAsync(6);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // ---------------------------------------------------------------
    // GetContactInfoAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetContactInfoAsync_RepositoryReturnsData_ReturnsSameData()
    {
        // Arrange
        var contactInfo = new ContactInfoDto { Email = "info@newshub.com", Phone = "+20123456789", Address = "Cairo, Egypt" };
        _mockSiteSettingsRepository.Setup(r => r.GetContactInfoAsync()).ReturnsAsync(contactInfo);

        // Act
        var result = await _service.GetContactInfoAsync();

        // Assert
        Assert.Equal("info@newshub.com", result.Email);
        Assert.Equal("+20123456789", result.Phone);
        Assert.Equal("Cairo, Egypt", result.Address);
    }

    [Fact]
    public async Task GetContactInfoAsync_CallsRepositoryGetContactInfoAsyncOnce()
    {
        // Arrange
        _mockSiteSettingsRepository.Setup(r => r.GetContactInfoAsync()).ReturnsAsync(new ContactInfoDto());

        // Act
        await _service.GetContactInfoAsync();

        // Assert
        _mockSiteSettingsRepository.Verify(r => r.GetContactInfoAsync(), Times.Once);
    }

    // ---------------------------------------------------------------
    // GetSocialLinksAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetSocialLinksAsync_RepositoryReturnsLinks_ReturnsSameData()
    {
        // Arrange
        var links = new List<SocialLinkDto>
        {
            new() { Platform = "Facebook", Url = "https://facebook.com/newshub" },
            new() { Platform = "Twitter", Url = "https://twitter.com/newshub" }
        };

        _mockSiteSettingsRepository.Setup(r => r.GetSocialLinksAsync()).ReturnsAsync(links);

        // Act
        var result = await _service.GetSocialLinksAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, l => l.Platform == "Facebook");
    }

    [Fact]
    public async Task GetSocialLinksAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        // Arrange
        _mockSiteSettingsRepository.Setup(r => r.GetSocialLinksAsync()).ReturnsAsync(new List<SocialLinkDto>());

        // Act
        var result = await _service.GetSocialLinksAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSocialLinksAsync_CallsRepositoryGetSocialLinksAsyncOnce()
    {
        // Arrange
        _mockSiteSettingsRepository.Setup(r => r.GetSocialLinksAsync()).ReturnsAsync(new List<SocialLinkDto>());

        // Act
        await _service.GetSocialLinksAsync();

        // Assert
        _mockSiteSettingsRepository.Verify(r => r.GetSocialLinksAsync(), Times.Once);
    }
}