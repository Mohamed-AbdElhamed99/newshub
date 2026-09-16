namespace NewsHub.Tests.ApplicationTests.Site.Services;

using Moq;
using NewsHub.Application.Common.Random;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

public class HomePageServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IArticleService> _mockArticleService;
    private readonly Mock<IRandomProvider> _mockRandomProvider;
    private readonly HomePageService _service;

    public HomePageServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockArticleService = new Mock<IArticleService>();
        _mockRandomProvider = new Mock<IRandomProvider>();
        _service = new HomePageService(_mockCategoryRepository.Object, _mockArticleService.Object, _mockRandomProvider.Object);
    }

    private static Category MakeCategory(int id, string slug, string name = "Category", string lang = "en")
    {
        return new Category
        {
            Id = id,
            Translations = new List<CategoryTranslation>
            {
                new() { LanguageCode = lang, Name = name, Slug = slug }
            }
        };
    }

    // ---------------------------------------------------------------
    // GetWhatIsNewAsync — unchanged tests from before still apply, omitted here for brevity
    // ---------------------------------------------------------------

    // ---------------------------------------------------------------
    // GetLifeStyleSectionAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetLifeStyleSectionAsync_LifeStyleCategoryExists_ReturnsItsArticles()
    {
        // Arrange
        var categories = new List<Category>
        {
            MakeCategory(1, "sports", "Sports"),
            MakeCategory(2, WellKnownCategorySlugs.LifeStyle, "Life Style")
        };

        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        _mockArticleService
            .Setup(s => s.GetByCategoryAsync(2, "Life Style", 2))
            .ReturnsAsync(new CategoryArticlesDto { CategoryId = 2, CategoryName = "Life Style", Articles = new List<LatestArticleDto> { new(), new() } });

        // Act
        var result = await _service.GetLifeStyleSectionAsync(2);

        // Assert
        Assert.Equal(2, result.CategoryId);
        Assert.Equal("Life Style", result.CategoryName);
        Assert.Equal(2, result.Articles.Count);
    }

    [Fact]
    public async Task GetLifeStyleSectionAsync_LifeStyleCategoryExists_NeverCallsRandomProvider()
    {
        // Arrange
        var categories = new List<Category> { MakeCategory(1, WellKnownCategorySlugs.LifeStyle, "Life Style") };
        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
        _mockArticleService.Setup(s => s.GetByCategoryAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new CategoryArticlesDto());

        // Act
        await _service.GetLifeStyleSectionAsync(2);

        // Assert
        _mockRandomProvider.Verify(r => r.Next(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetLifeStyleSectionAsync_LifeStyleCategoryMissing_FallsBackToRandomCategory()
    {
        // Arrange
        var categories = new List<Category>
        {
            MakeCategory(1, "sports", "Sports"),
            MakeCategory(2, "tech", "Tech")
        };

        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
        _mockRandomProvider.Setup(r => r.Next(2)).Returns(1); // picks index 1 → Tech

        _mockArticleService
            .Setup(s => s.GetByCategoryAsync(2, "Tech", 2))
            .ReturnsAsync(new CategoryArticlesDto { CategoryId = 2, CategoryName = "Tech", Articles = new List<LatestArticleDto> { new(), new() } });

        // Act
        var result = await _service.GetLifeStyleSectionAsync(2);

        // Assert
        Assert.Equal(2, result.CategoryId);
        Assert.Equal("Tech", result.CategoryName);
    }

    [Fact]
    public async Task GetLifeStyleSectionAsync_FallbackPath_CallsRandomProviderWithCategoryCount()
    {
        // Arrange
        var categories = new List<Category> { MakeCategory(1, "sports"), MakeCategory(2, "tech"), MakeCategory(3, "news") };
        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
        _mockRandomProvider.Setup(r => r.Next(It.IsAny<int>())).Returns(0);
        _mockArticleService.Setup(s => s.GetByCategoryAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(new CategoryArticlesDto());

        // Act
        await _service.GetLifeStyleSectionAsync(2);

        // Assert
        _mockRandomProvider.Verify(r => r.Next(3), Times.Once);
    }

    [Fact]
    public async Task GetLifeStyleSectionAsync_NoCategoriesExist_ReturnsEmptyPlaceholderDto()
    {
        // Arrange
        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

        // Act
        var result = await _service.GetLifeStyleSectionAsync(2);

        // Assert
        Assert.Equal(0, result.CategoryId);
        Assert.Empty(result.Articles);
        _mockArticleService.Verify(s => s.GetByCategoryAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockRandomProvider.Verify(r => r.Next(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetLifeStyleSectionAsync_MultipleCategoriesMatchLifeStyleSlugPattern_UsesFirstMatch()
    {
        // Arrange — defensive test: even if data integrity is somehow violated (two categories with the same slug,
        // which shouldn't happen given a unique constraint), the service picks deterministically rather than throwing.
        var categories = new List<Category>
        {
            MakeCategory(1, WellKnownCategorySlugs.LifeStyle, "Life Style A"),
            MakeCategory(2, WellKnownCategorySlugs.LifeStyle, "Life Style B")
        };

        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
        _mockArticleService.Setup(s => s.GetByCategoryAsync(1, "Life Style A", 2))
            .ReturnsAsync(new CategoryArticlesDto { CategoryId = 1, CategoryName = "Life Style A" });

        // Act
        var result = await _service.GetLifeStyleSectionAsync(2);

        // Assert
        Assert.Equal(1, result.CategoryId);
    }

    [Fact]
    public async Task GetLifeStyleSectionAsync_ValidCount_PassesCountThroughToArticleService()
    {
        // Arrange
        var categories = new List<Category> { MakeCategory(1, WellKnownCategorySlugs.LifeStyle, "Life Style") };
        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
        _mockArticleService.Setup(s => s.GetByCategoryAsync(1, "Life Style", 2))
            .ReturnsAsync(new CategoryArticlesDto());

        // Act
        await _service.GetLifeStyleSectionAsync(2);

        // Assert
        _mockArticleService.Verify(s => s.GetByCategoryAsync(1, "Life Style", 2), Times.Once);
    }
}