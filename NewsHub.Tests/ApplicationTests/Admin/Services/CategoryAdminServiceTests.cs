using System.Globalization;
using Moq;
using NewsHub.Application.Admin.DTOs.Categories;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Admin.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.ApplicationTests.Admin.Services;

public class CategoryAdminServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CategoryAdminService _service;
    
    public CategoryAdminServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _service = new CategoryAdminService(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidDto_ReturnCorrectlyMappedDto()
    {
        //Arrange
        var dto = new CreateCategoryDto
        {
            ImagePath = "test.jpg",
            Translations = new List<CategoryTranslationDto>
            {
                new () {LanguageCode = "en" , Name = "Test" , Slug = "test" },
                new () {LanguageCode = "ar" , Name = "تست" , Slug = "تست" }
            }
        };
        
        _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
        
        // Act
        var result = await _service.CreateCategoryAsync(dto);
        
        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidDto_CallsRepositoryAddAsyncOnce()
    {
        //Arrange
        var dto = new CreateCategoryDto
        {
            ImagePath = "test.jpg",
            Translations = new List<CategoryTranslationDto>
            {
                new () {LanguageCode = "en" , Name = "Test" , Slug = "test" },
                new () {LanguageCode = "ar" , Name = "تست" , Slug = "tst" }
            }
        };
        
        // Act
        var result = await _service.CreateCategoryAsync(dto);
        
        //Assert
        _mockCategoryRepository.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_ValidDto_PassesCorrectDataToRepository()
    {
        //Arrange 
        Category? capturedCategory = null;
        var dto = new CreateCategoryDto
        {
            ImagePath = "/images/sports.png",
            Translations = new List<CategoryTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Sports", Slug = "sports", Description = "Sports news" }
            }
        };
        
        _mockCategoryRepository.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .Callback<Category>(c => capturedCategory = c)
            .Returns(Task.CompletedTask);
        
        // Act
        await _service.CreateCategoryAsync(dto);
        
        // Assert
        Assert.NotNull(capturedCategory);
        Assert.Equal(dto.ImagePath, capturedCategory!.ImagePath);
        Assert.Single(capturedCategory.Translations);
        Assert.Equal("Sports", capturedCategory.Translations.First().Name);
    }

    [Fact]
    public async Task CreateCategoryAsync_MultipleTranslations_MapsAllTranslationsCorrectly()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            ImagePath = "/images/news.png",
            Translations = new List<CategoryTranslationDto>
            {
                new() { LanguageCode = "en", Name = "News", Slug = "news", Description = "General news" },
                new() { LanguageCode = "ar", Name = "أخبار", Slug = "akhbar", Description = "أخبار عامة" }
            }
        };
        
        // Act
        var result = await _service.CreateCategoryAsync(dto);
        
        //Assert
        Assert.Equal(2, result.Translations.Count);
        Assert.Contains(result.Translations, t => t.LanguageCode == "en" && t.Name == "News");
        Assert.Contains(result.Translations, t => t.LanguageCode == "ar" && t.Name == "أخبار");
    }
    
    [Fact]
    public async Task CreateCategoryAsync_EmptyTranslationsList_ReturnsDtoWithNoTranslations()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            ImagePath = "/images/empty.png",
            Translations = new List<CategoryTranslationDto>()
        };

        // Act
        var result = await _service.CreateCategoryAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Translations);
    }
    
    [Fact]
    public async Task CreateCategoryAsync_NullImagePath_ReturnsDtoWithNullImagePath()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            ImagePath = null,
            Translations = new List<CategoryTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Test", Slug = "test", Description = "Test desc" }
            }
        };

        // Act
        var result = await _service.CreateCategoryAsync(dto);

        // Assert
        Assert.Null(result.ImagePath);
    }

    [Fact]
public async Task GetCategoryByIdAsync_CategoryExists_ReturnsCorrectlyMappedDto()
{
    // Arrange
    var category = new Category
    {
        Id = 1,
        ImagePath = "/images/tech.png",
        Translations = new List<CategoryTranslation>
        {
            new() { LanguageCode = "en", Name = "Technology", Slug = "technology", Description = "Tech news" },
            new() { LanguageCode = "ar", Name = "تكنولوجيا", Slug = "tech-ar", Description = "أخبار تقنية" }
        }
    };

    _mockCategoryRepository
        .Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(category);

    // Act
    var result = await _service.GetCategoryByIdAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(1, result!.Id);
    Assert.Equal("/images/tech.png", result.ImagePath);
    Assert.Equal(2, result.Translations.Count);
    Assert.Contains(result.Translations, t => t.LanguageCode == "en" && t.Name == "Technology");
    Assert.Contains(result.Translations, t => t.LanguageCode == "ar" && t.Name == "تكنولوجيا");
}

[Fact]
public async Task GetCategoryByIdAsync_CategoryNotFound_ReturnsNull()
{
    // Arrange
    _mockCategoryRepository
        .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
        .ReturnsAsync((Category?)null);

    // Act
    var result = await _service.GetCategoryByIdAsync(999);

    // Assert
    Assert.Null(result);
}

[Fact]
public async Task GetCategoryByIdAsync_CategoryExists_CallsRepositoryGetByIdAsyncWithCorrectId()
{
    // Arrange
    var category = new Category
    {
        Id = 5,
        Translations = new List<CategoryTranslation>
        {
            new() { LanguageCode = "en", Name = "Sports", Slug = "sports", Description = "Sports news" }
        }
    };

    _mockCategoryRepository
        .Setup(r => r.GetByIdAsync(5))
        .ReturnsAsync(category);

    // Act
    await _service.GetCategoryByIdAsync(5);

    // Assert
    _mockCategoryRepository.Verify(r => r.GetByIdAsync(5), Times.Once);
}

// ---------------------------------------------------------------

[Fact]
public async Task GetCategoriesAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
{
    // Arrange
    var categories = new List<Category>
    {
        new()
        {
            Id = 1,
            Translations = new List<CategoryTranslation>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech", Description = "Tech news" }
            }
        }
    };

    var filter = new CategoryFilter { Page = 2, PageSize = 5 };

    _mockCategoryRepository
        .Setup(r => r.GetAllAsync(filter))
        .ReturnsAsync((categories, 17));

    // Act
    var result = await _service.GetCategoriesAsync(filter);

    // Assert
    Assert.Equal(17, result.TotalCount);
    Assert.Equal(2, result.Page);
    Assert.Equal(5, result.PageSize);
    Assert.Single(result.Items);
}

[Fact]
public async Task GetCategoriesAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
{
    // Arrange
    var filter = new CategoryFilter { Page = 1, PageSize = 10 };

    _mockCategoryRepository
        .Setup(r => r.GetAllAsync(filter))
        .ReturnsAsync((new List<Category>(), 0));

    // Act
    var result = await _service.GetCategoriesAsync(filter);

    // Assert
    Assert.NotNull(result.Items);
    Assert.Empty(result.Items);
    Assert.Equal(0, result.TotalCount);
}

[Fact]
public async Task GetCategoriesAsync_MultipleCategories_MapsEachToCorrectDto()
{
    // Arrange
    var categories = new List<Category>
    {
        new()
        {
            Id = 1,
            ImagePath = "/images/tech.png",
            Translations = new List<CategoryTranslation>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech", Description = "Tech news" }
            }
        },
        new()
        {
            Id = 2,
            ImagePath = "/images/sports.png",
            Translations = new List<CategoryTranslation>
            {
                new() { LanguageCode = "en", Name = "Sports", Slug = "sports", Description = "Sports news" }
            }
        }
    };

    var filter = new CategoryFilter { Page = 1, PageSize = 10 };

    _mockCategoryRepository
        .Setup(r => r.GetAllAsync(filter))
        .ReturnsAsync((categories, 2));

    // Act
    var result = await _service.GetCategoriesAsync(filter);

    // Assert
    Assert.Equal(2, result.Items.Count);
    Assert.Contains(result.Items, i => i.Id == 1 && i.Name == "Tech");
    Assert.Contains(result.Items, i => i.Id == 2 && i.Name == "Sports");
}

// --- MapToListDto fallback chain (exercised via GetCategoriesAsync) ---

[Fact]
public async Task GetCategoriesAsync_TranslationMatchesCurrentCulture_UsesExactMatch()
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
                    new() { LanguageCode = "en", Name = "Tech", Slug = "tech", Description = "Tech news" },
                    new() { LanguageCode = "fr", Name = "Technologie", Slug = "tech-fr", Description = "Actualités tech" }
                }
            }
        };

        var filter = new CategoryFilter { Page = 1, PageSize = 10 };

        _mockCategoryRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((categories, 1));

        // Act
        var result = await _service.GetCategoriesAsync(filter);

        // Assert
        Assert.Equal("Technologie", result.Items.First().Name);
        Assert.Equal("fr", result.Items.First().LanguageCode);
    }
    finally
    {
        CultureInfo.CurrentCulture = originalCulture;
    }
}

[Fact]
public async Task GetCategoriesAsync_NoCultureMatch_FallsBackToEnglish()
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
                    new() { LanguageCode = "en", Name = "Tech", Slug = "tech", Description = "Tech news" },
                    new() { LanguageCode = "ar", Name = "تكنولوجيا", Slug = "tech-ar", Description = "أخبار تقنية" }
                }
            }
        };

        var filter = new CategoryFilter { Page = 1, PageSize = 10 };

        _mockCategoryRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((categories, 1));

        // Act
        var result = await _service.GetCategoriesAsync(filter);

        // Assert
        Assert.Equal("Tech", result.Items.First().Name);
        Assert.Equal("en", result.Items.First().LanguageCode);
    }
    finally
    {
        CultureInfo.CurrentCulture = originalCulture;
    }
}

[Fact]
public async Task GetCategoriesAsync_NoCultureOrEnglishMatch_FallsBackToFirstTranslation()
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
                    new() { LanguageCode = "ar", Name = "تكنولوجيا", Slug = "tech-ar", Description = "أخبار تقنية" },
                    new() { LanguageCode = "de", Name = "Technologie", Slug = "tech-de", Description = "Tech Nachrichten" }
                }
            }
        };

        var filter = new CategoryFilter { Page = 1, PageSize = 10 };

        _mockCategoryRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((categories, 1));

        // Act
        var result = await _service.GetCategoriesAsync(filter);

        // Assert — falls back to whatever .First() returns (first item in the list, "ar" here)
        Assert.Equal("تكنولوجيا", result.Items.First().Name);
        Assert.Equal("ar", result.Items.First().LanguageCode);
    }
    finally
    {
        CultureInfo.CurrentCulture = originalCulture;
    }
}
}