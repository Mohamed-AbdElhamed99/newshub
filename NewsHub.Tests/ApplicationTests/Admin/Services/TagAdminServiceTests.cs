using System.Globalization;

namespace NewsHub.Tests.ApplicationTests.Admin.Services;

using Moq;
using NewsHub.Application.Admin.DTOs.Tags;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Admin.Services;
using NewsHub.Domain.Entities;

public class TagAdminServiceTests
{
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly TagAdminService _service;

    public TagAdminServiceTests()
    {
        _mockTagRepository = new Mock<ITagRepository>();
        _service = new TagAdminService(_mockTagRepository.Object);
    }

    [Fact]
    public async Task CreateTagAsync_ValidDto_ReturnCorrectlyMappedDto()
    {
        //Arrange
        var dto = new CreateTagDto
        {
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Football", Slug = "football" },
                new() { LanguageCode = "ar", Name = "كرة قدم", Slug = "كرة-قدم" }
            }
        };

        _mockTagRepository.Setup(r => r.AddAsync(It.IsAny<Tag>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateTagAsync(dto);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateTagAsync_ValidDto_CallsRepositoryAddAsyncOnce()
    {
        //Arrange
        var dto = new CreateTagDto
        {
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Football", Slug = "football" }
            }
        };

        // Act
        var result = await _service.CreateTagAsync(dto);

        //Assert
        _mockTagRepository.Verify(r => r.AddAsync(It.IsAny<Tag>()), Times.Once);
    }

    [Fact]
    public async Task CreateTagAsync_ValidDto_PassesCorrectDataToRepository()
    {
        //Arrange
        Tag? capturedTag = null;
        var dto = new CreateTagDto
        {
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Basketball", Slug = "basketball" }
            }
        };

        _mockTagRepository.Setup(r => r.AddAsync(It.IsAny<Tag>()))
            .Callback<Tag>(t => capturedTag = t)
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateTagAsync(dto);

        // Assert
        Assert.NotNull(capturedTag);
        Assert.Single(capturedTag!.Translations);
        Assert.Equal("Basketball", capturedTag.Translations.First().Name);
    }

    [Fact]
    public async Task CreateTagAsync_MultipleTranslations_MapsAllTranslationsCorrectly()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech" },
                new() { LanguageCode = "ar", Name = "تقنية", Slug = "taqniya" }
            }
        };

        // Act
        var result = await _service.CreateTagAsync(dto);

        //Assert
        Assert.Equal(2, result.Translations.Count);
        Assert.Contains(result.Translations, t => t.LanguageCode == "en" && t.Name == "Tech");
        Assert.Contains(result.Translations, t => t.LanguageCode == "ar" && t.Name == "تقنية");
    }

    [Fact]
    public async Task CreateTagAsync_EmptyTranslationsList_ReturnsDtoWithNoTranslations()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Translations = new List<TagTranslationDto>()
        };

        // Act
        var result = await _service.CreateTagAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Translations);
    }

    [Fact]
    public async Task GetTagByIdAsync_TagExists_ReturnsCorrectlyMappedDto()
    {
        // Arrange
        var tag = new Tag
        {
            Id = 1,
            Translations = new List<TagTranslation>
            {
                new() { LanguageCode = "en", Name = "Football", Slug = "football" },
                new() { LanguageCode = "ar", Name = "كرة قدم", Slug = "korat-qadam" }
            }
        };

        _mockTagRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(tag);

        // Act
        var result = await _service.GetTagByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal(2, result.Translations.Count);
        Assert.Contains(result.Translations, t => t.LanguageCode == "en" && t.Name == "Football");
    }

    [Fact]
    public async Task GetTagByIdAsync_TagNotFound_ReturnsNull()
    {
        // Arrange
        _mockTagRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Tag?)null);

        // Act
        var result = await _service.GetTagByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTagByIdAsync_TagExists_CallsRepositoryGetByIdAsyncWithCorrectId()
    {
        // Arrange
        var tag = new Tag
        {
            Id = 5,
            Translations = new List<TagTranslation>
            {
                new() { LanguageCode = "en", Name = "Sports", Slug = "sports" }
            }
        };

        _mockTagRepository
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(tag);

        // Act
        await _service.GetTagByIdAsync(5);

        // Assert
        _mockTagRepository.Verify(r => r.GetByIdAsync(5), Times.Once);
    }

    // ---------------------------------------------------------------

    [Fact]
    public async Task GetTagsAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new()
            {
                Id = 1,
                Translations = new List<TagTranslation>
                {
                    new() { LanguageCode = "en", Name = "Tech", Slug = "tech" }
                }
            }
        };

        var filter = new TagFilter { Page = 2, PageSize = 5 };

        _mockTagRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((tags, 17));

        // Act
        var result = await _service.GetTagsAsync(filter);

        // Assert
        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetTagsAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
    {
        // Arrange
        var filter = new TagFilter { Page = 1, PageSize = 10 };

        _mockTagRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<Tag>(), 0));

        // Act
        var result = await _service.GetTagsAsync(filter);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetTagsAsync_MultipleTags_MapsEachToCorrectDto()
    {
        // Arrange
        var tags = new List<Tag>
        {
            new()
            {
                Id = 1,
                Translations = new List<TagTranslation>
                {
                    new() { LanguageCode = "en", Name = "Tech", Slug = "tech" }
                }
            },
            new()
            {
                Id = 2,
                Translations = new List<TagTranslation>
                {
                    new() { LanguageCode = "en", Name = "Sports", Slug = "sports" }
                }
            }
        };

        var filter = new TagFilter { Page = 1, PageSize = 10 };

        _mockTagRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((tags, 2));

        // Act
        var result = await _service.GetTagsAsync(filter);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Id == 1 && i.Name == "Tech");
        Assert.Contains(result.Items, i => i.Id == 2 && i.Name == "Sports");
    }

    // --- MapToListDto fallback chain (exercised via GetTagsAsync) ---

    [Fact]
    public async Task GetTagsAsync_TranslationMatchesCurrentCulture_UsesExactMatch()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var tags = new List<Tag>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<TagTranslation>
                    {
                        new() { LanguageCode = "en", Name = "Tech", Slug = "tech" },
                        new() { LanguageCode = "fr", Name = "Technologie", Slug = "tech-fr" }
                    }
                }
            };

            var filter = new TagFilter { Page = 1, PageSize = 10 };

            _mockTagRepository
                .Setup(r => r.GetAllAsync(filter))
                .ReturnsAsync((tags, 1));

            // Act
            var result = await _service.GetTagsAsync(filter);

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
    public async Task GetTagsAsync_NoCultureMatch_FallsBackToEnglish()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var tags = new List<Tag>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<TagTranslation>
                    {
                        new() { LanguageCode = "en", Name = "Tech", Slug = "tech" },
                        new() { LanguageCode = "ar", Name = "تقنية", Slug = "taqniya" }
                    }
                }
            };

            var filter = new TagFilter { Page = 1, PageSize = 10 };

            _mockTagRepository
                .Setup(r => r.GetAllAsync(filter))
                .ReturnsAsync((tags, 1));

            // Act
            var result = await _service.GetTagsAsync(filter);

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
    public async Task GetTagsAsync_NoCultureOrEnglishMatch_FallsBackToFirstTranslation()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var tags = new List<Tag>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<TagTranslation>
                    {
                        new() { LanguageCode = "ar", Name = "تقنية", Slug = "taqniya" },
                        new() { LanguageCode = "de", Name = "Technologie", Slug = "tech-de" }
                    }
                }
            };

            var filter = new TagFilter { Page = 1, PageSize = 10 };

            _mockTagRepository
                .Setup(r => r.GetAllAsync(filter))
                .ReturnsAsync((tags, 1));

            // Act
            var result = await _service.GetTagsAsync(filter);

            // Assert — falls back to whatever .First() returns (first item in the list, "ar" here)
            Assert.Equal("تقنية", result.Items.First().Name);
            Assert.Equal("ar", result.Items.First().LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    // ---------------------------------------------------------------

    [Fact]
    public async Task UpdateTagAsync_TagNotFound_ReturnsNull()
    {
        // Arrange
        _mockTagRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Tag?)null);

        var dto = new UpdateTagDto { Id = 999, Translations = new List<TagTranslationDto>() };

        // Act
        var result = await _service.UpdateTagAsync(dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateTagAsync_ExistingTranslation_UpdatesFieldsInPlace()
    {
        // Arrange
        var tag = new Tag
        {
            Id = 1,
            Translations = new List<TagTranslation>
            {
                new() { LanguageCode = "en", Name = "Old Name", Slug = "old-slug" }
            }
        };

        _mockTagRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);

        var dto = new UpdateTagDto
        {
            Id = 1,
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "New Name", Slug = "new-slug" }
            }
        };

        // Act
        var result = await _service.UpdateTagAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result!.Translations);
        Assert.Equal("New Name", result.Translations.First().Name);
        Assert.Equal("new-slug", result.Translations.First().Slug);
    }

    [Fact]
    public async Task UpdateTagAsync_NewLanguageCode_AddsNewTranslation()
    {
        // Arrange
        var tag = new Tag
        {
            Id = 1,
            Translations = new List<TagTranslation>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech" }
            }
        };

        _mockTagRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);

        var dto = new UpdateTagDto
        {
            Id = 1,
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech" },
                new() { LanguageCode = "ar", Name = "تقنية", Slug = "taqniya" }
            }
        };

        // Act
        var result = await _service.UpdateTagAsync(dto);

        // Assert
        Assert.Equal(2, result!.Translations.Count);
        Assert.Contains(result.Translations, t => t.LanguageCode == "ar" && t.Name == "تقنية");
    }

    [Fact]
    public async Task UpdateTagAsync_MissingLanguageCodeInDto_RemovesTranslation()
    {
        // Arrange
        var tag = new Tag
        {
            Id = 1,
            Translations = new List<TagTranslation>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech" },
                new() { LanguageCode = "ar", Name = "تقنية", Slug = "taqniya" }
            }
        };

        _mockTagRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);

        var dto = new UpdateTagDto
        {
            Id = 1,
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech" }
            }
        };

        // Act
        var result = await _service.UpdateTagAsync(dto);

        // Assert
        Assert.Single(result!.Translations);
        Assert.DoesNotContain(result.Translations, t => t.LanguageCode == "ar");
    }

    [Fact]
    public async Task UpdateTagAsync_ValidDto_CallsRepositoryUpdateAsyncOnce()
    {
        // Arrange
        var tag = new Tag
        {
            Id = 1,
            Translations = new List<TagTranslation>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech" }
            }
        };

        _mockTagRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);

        var dto = new UpdateTagDto
        {
            Id = 1,
            Translations = new List<TagTranslationDto>
            {
                new() { LanguageCode = "en", Name = "Tech", Slug = "tech" }
            }
        };

        // Act
        await _service.UpdateTagAsync(dto);

        // Assert
        _mockTagRepository.Verify(r => r.UpdateAsync(tag), Times.Once);
    }

    // ---------------------------------------------------------------

    [Fact]
    public async Task DeleteTagAsync_TagExists_ReturnsTrueAndCallsDeleteAsync()
    {
        // Arrange
        var tag = new Tag { Id = 1 };
        _mockTagRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tag);

        // Act
        var result = await _service.DeleteTagAsync(1);

        // Assert
        Assert.True(result);
        _mockTagRepository.Verify(r => r.DeleteAsync(tag), Times.Once);
    }

    [Fact]
    public async Task DeleteTagAsync_TagNotFound_ReturnsFalse()
    {
        // Arrange
        _mockTagRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Tag?)null);

        // Act
        var result = await _service.DeleteTagAsync(999);

        // Assert
        Assert.False(result);
        _mockTagRepository.Verify(r => r.DeleteAsync(It.IsAny<Tag>()), Times.Never);
    }
}