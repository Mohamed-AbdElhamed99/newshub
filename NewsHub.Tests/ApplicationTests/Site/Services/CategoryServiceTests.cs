namespace NewsHub.Tests.ApplicationTests.Site.Services;

using System.Globalization;
using Moq;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockCategoryRepository.Object);
    }

    // ---------------------------------------------------------------
    // GetAllAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetAllAsync_RepositoryReturnsCategories_MapsToListItemDtoCorrectly()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                Id = 1,
                ImagePath = "/images/sports.png",
                CreatedAt = new DateTime(2026, 1, 1),
                Translations = new List<CategoryTranslation>
                {
                    new() { LanguageCode = "en", Name = "Sports", Description = "All things sports", Slug = "sports" }
                }
            }
        };

        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Sports", dto.Name);
        Assert.Equal("All things sports", dto.Description);
        Assert.Equal("/images/sports.png", dto.ImagePath);
        Assert.Equal(new DateTime(2026, 1, 1), dto.CreatedAt);
        Assert.Equal("sports", dto.Slug);
    }

    [Fact]
    public async Task GetAllAsync_CallsRepositoryGetAllAsyncOnce()
    {
        // Arrange
        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

        // Act
        await _service.GetAllAsync();

        // Assert
        _mockCategoryRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        // Arrange
        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Category>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_MultipleCategories_MapsEachCorrectly()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Translations = new List<CategoryTranslation> { new() { LanguageCode = "en", Name = "Sports", Slug = "sports" } } },
            new() { Id = 2, Translations = new List<CategoryTranslation> { new() { LanguageCode = "en", Name = "Tech", Slug = "tech" } } }
        };

        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Id == 1 && c.Name == "Sports");
        Assert.Contains(result, c => c.Id == 2 && c.Name == "Tech");
    }

    [Fact]
    public async Task GetAllAsync_CategoryHasNullDescriptionOrImagePath_MapsAsNull()
    {
        // Arrange
        var categories = new List<Category>
        {
            new()
            {
                Id = 1,
                ImagePath = null,
                Translations = new List<CategoryTranslation>
                {
                    new() { LanguageCode = "en", Name = "Sports", Description = null, Slug = "sports" }
                }
            }
        };

        _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Null(result.First().Description);
        Assert.Null(result.First().ImagePath);
    }

    // --- translation fallback chain ---

    [Fact]
    public async Task GetAllAsync_TranslationMatchesCurrentCulture_UsesExactMatch()
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
                        new() { LanguageCode = "fr", Name = "Sport", Slug = "sport-fr" }
                    }
                }
            };

            _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal("Sport", result.First().Name);
            Assert.Equal("fr", result.First().LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public async Task GetAllAsync_NoCultureMatch_FallsBackToEnglish()
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

            _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal("Sports", result.First().Name);
            Assert.Equal("en", result.First().LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public async Task GetAllAsync_NoCultureOrEnglishMatch_FallsBackToFirstTranslation()
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

            _mockCategoryRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal("رياضة", result.First().Name);
            Assert.Equal("ar", result.First().LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }
}