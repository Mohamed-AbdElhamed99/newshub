namespace NewsHub.Tests.ApplicationTests.Admin.Services;

using System.Globalization;
using Moq;
using NewsHub.Application.Admin.DTOs.Articles;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Admin.Services;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;

public class ArticleAdminServiceTests
{
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly ArticleAdminService _service;

    public ArticleAdminServiceTests()
    {   
        _mockArticleRepository = new Mock<IArticleRepository>();
        _service = new ArticleAdminService(_mockArticleRepository.Object);
    }

    // ---------------------------------------------------------------
    // CreateArticleAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task CreateArticleAsync_ValidDto_ReturnCorrectlyMappedDto()
    {
        //Arrange
        var dto = new CreateArticleDto
        {
            AuthorId = Guid.NewGuid(),
            CategoryId = 3,
            ImageUrl = "test.jpg",
            TagIds = new List<int> { 1, 2 },
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Test", Slug = "test", Content = "Content" },
                new() { LanguageCode = "ar", Title = "تست", Slug = "تست", Content = "محتوى" }
            }
        };

        _mockArticleRepository.Setup(r => r.AddAsync(It.IsAny<Article>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateArticleAsync(dto);

        //Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreateArticleAsync_ValidDto_CallsRepositoryAddAsyncOnce()
    {
        //Arrange
        var dto = new CreateArticleDto
        {
            AuthorId = Guid.NewGuid(),
            CategoryId = 3,
            ImageUrl = "test.jpg",
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Test", Slug = "test", Content = "Content" }
            }
        };

        // Act
        await _service.CreateArticleAsync(dto);

        //Assert
        _mockArticleRepository.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task CreateArticleAsync_ValidDto_PassesCorrectDataToRepository()
    {
        //Arrange
        Article? capturedArticle = null;
        var dto = new CreateArticleDto
        {
            AuthorId = Guid.NewGuid(),
            CategoryId = 7,
            ImageUrl = "/images/sports.png",
            TagIds = new List<int> { 1 },
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Sports News", Slug = "sports-news", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.AddAsync(It.IsAny<Article>()))
            .Callback<Article>(a => capturedArticle = a)
            .Returns(Task.CompletedTask);

        // Act
        await _service.CreateArticleAsync(dto);

        // Assert
        Assert.NotNull(capturedArticle);
        Assert.Equal(dto.AuthorId, capturedArticle!.AuthorId);
        Assert.Equal(dto.CategoryId, capturedArticle.CategoryId);
        Assert.Equal(dto.ImageUrl, capturedArticle.ImageUrl);
        Assert.Single(capturedArticle.Translations);
        Assert.Equal("Sports News", capturedArticle.Translations.First().Title);
        Assert.Single(capturedArticle.Tags);
        Assert.Equal(1, capturedArticle.Tags.First().TagId);
    }

    [Fact]
    public async Task CreateArticleAsync_ValidDto_DefaultsStatusToDraft()
    {
        //Arrange
        Article? capturedArticle = null;
        var dto = new CreateArticleDto
        {
            AuthorId = Guid.NewGuid(),
            CategoryId = 1,
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Test", Slug = "test", Content = "Content" }
            }
        };

        _mockArticleRepository.Setup(r => r.AddAsync(It.IsAny<Article>()))
            .Callback<Article>(a => capturedArticle = a)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateArticleAsync(dto);

        // Assert
        Assert.Equal(ArticleStatus.Draft, capturedArticle!.Status);
        Assert.Equal(ArticleStatus.Draft, result.Status);
    }

    [Fact]
    public async Task CreateArticleAsync_MultipleTranslations_MapsAllTranslationsCorrectly()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            AuthorId = Guid.NewGuid(),
            CategoryId = 1,
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "News", Slug = "news", Content = "Content EN" },
                new() { LanguageCode = "ar", Title = "أخبار", Slug = "akhbar", Content = "محتوى" }
            }
        };

        // Act
        var result = await _service.CreateArticleAsync(dto);

        //Assert
        Assert.Equal(2, result.Translations.Count);
        Assert.Contains(result.Translations, t => t.LanguageCode == "en" && t.Title == "News");
        Assert.Contains(result.Translations, t => t.LanguageCode == "ar" && t.Title == "أخبار");
    }

    [Fact]
    public async Task CreateArticleAsync_NoTagIds_ReturnsDtoWithEmptyTagIds()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            AuthorId = Guid.NewGuid(),
            CategoryId = 1,
            TagIds = new List<int>(),
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Test", Slug = "test", Content = "Content" }
            }
        };

        // Act
        var result = await _service.CreateArticleAsync(dto);

        // Assert
        Assert.NotNull(result.TagIds);
        Assert.Empty(result.TagIds);
    }

    [Fact]
    public async Task CreateArticleAsync_WithTagIds_MapsTagIdsCorrectly()
    {
        // Arrange
        var dto = new CreateArticleDto
        {
            AuthorId = Guid.NewGuid(),
            CategoryId = 1,
            TagIds = new List<int> { 4, 9 },
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Test", Slug = "test", Content = "Content" }
            }
        };

        // Act
        var result = await _service.CreateArticleAsync(dto);

        // Assert
        Assert.Equal(2, result.TagIds.Count);
        Assert.Contains(4, result.TagIds);
        Assert.Contains(9, result.TagIds);
    }

    // ---------------------------------------------------------------
    // GetArticleByIdAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetArticleByIdAsync_ArticleExists_ReturnsCorrectlyMappedDto()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            AuthorId = Guid.NewGuid(),
            CategoryId = 3,
            ImageUrl = "/images/tech.png",
            Tags = new List<ArticleTag> { new() { TagId = 1 }, new() { TagId = 2 } },
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Technology", Slug = "technology", Content = "Body" },
                new() { LanguageCode = "ar", Title = "تكنولوجيا", Slug = "tech-ar", Content = "محتوى" }
            }
        };

        _mockArticleRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(article);

        // Act
        var result = await _service.GetArticleByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("/images/tech.png", result.ImageUrl);
        Assert.Equal(2, result.Translations.Count);
        Assert.Equal(2, result.TagIds.Count);
        Assert.Contains(result.Translations, t => t.LanguageCode == "en" && t.Title == "Technology");
    }

    [Fact]
    public async Task GetArticleByIdAsync_ArticleNotFound_ReturnsNull()
    {
        // Arrange
        _mockArticleRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Article?)null);

        // Act
        var result = await _service.GetArticleByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetArticleByIdAsync_ArticleExists_CallsRepositoryGetByIdAsyncWithCorrectId()
    {
        // Arrange
        var article = new Article
        {
            Id = 5,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Sports", Slug = "sports", Content = "Body" }
            }
        };

        _mockArticleRepository
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(article);

        // Act
        await _service.GetArticleByIdAsync(5);

        // Assert
        _mockArticleRepository.Verify(r => r.GetByIdAsync(5), Times.Once);
    }

    // ---------------------------------------------------------------
    // GetArticlesAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetArticlesAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
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

        var filter = new ArticleFilter { Page = 2, PageSize = 5 };

        _mockArticleRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((articles, 17));

        // Act
        var result = await _service.GetArticlesAsync(filter);

        // Assert
        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetArticlesAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
    {
        // Arrange
        var filter = new ArticleFilter { Page = 1, PageSize = 10 };

        _mockArticleRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((new List<Article>(), 0));

        // Act
        var result = await _service.GetArticlesAsync(filter);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetArticlesAsync_MultipleArticles_MapsEachToCorrectDto()
    {
        // Arrange
        var articles = new List<Article>
        {
            new()
            {
                Id = 1,
                CategoryId = 3,
                ImageUrl = "/images/tech.png",
                Translations = new List<ArticleTranslation>
                {
                    new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
                }
            },
            new()
            {
                Id = 2,
                CategoryId = 4,
                ImageUrl = "/images/sports.png",
                Translations = new List<ArticleTranslation>
                {
                    new() { LanguageCode = "en", Title = "Sports", Slug = "sports", Content = "Body" }
                }
            }
        };

        var filter = new ArticleFilter { Page = 1, PageSize = 10 };

        _mockArticleRepository
            .Setup(r => r.GetAllAsync(filter))
            .ReturnsAsync((articles, 2));

        // Act
        var result = await _service.GetArticlesAsync(filter);

        // Assert
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Id == 1 && i.Title == "Tech");
        Assert.Contains(result.Items, i => i.Id == 2 && i.Title == "Sports");
    }

    // --- MapToListDto fallback chain (exercised via GetArticlesAsync) ---

    [Fact]
    public async Task GetArticlesAsync_TranslationMatchesCurrentCulture_UsesExactMatch()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var articles = new List<Article>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<ArticleTranslation>
                    {
                        new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" },
                        new() { LanguageCode = "fr", Title = "Technologie", Slug = "tech-fr", Content = "Contenu" }
                    }
                }
            };

            var filter = new ArticleFilter { Page = 1, PageSize = 10 };

            _mockArticleRepository
                .Setup(r => r.GetAllAsync(filter))
                .ReturnsAsync((articles, 1));

            // Act
            var result = await _service.GetArticlesAsync(filter);

            // Assert
            Assert.Equal("Technologie", result.Items.First().Title);
            Assert.Equal("fr", result.Items.First().LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public async Task GetArticlesAsync_NoCultureMatch_FallsBackToEnglish()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var articles = new List<Article>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<ArticleTranslation>
                    {
                        new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" },
                        new() { LanguageCode = "ar", Title = "تكنولوجيا", Slug = "tech-ar", Content = "محتوى" }
                    }
                }
            };

            var filter = new ArticleFilter { Page = 1, PageSize = 10 };

            _mockArticleRepository
                .Setup(r => r.GetAllAsync(filter))
                .ReturnsAsync((articles, 1));

            // Act
            var result = await _service.GetArticlesAsync(filter);

            // Assert
            Assert.Equal("Tech", result.Items.First().Title);
            Assert.Equal("en", result.Items.First().LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    [Fact]
    public async Task GetArticlesAsync_NoCultureOrEnglishMatch_FallsBackToFirstTranslation()
    {
        // Arrange
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");

        try
        {
            var articles = new List<Article>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<ArticleTranslation>
                    {
                        new() { LanguageCode = "ar", Title = "تكنولوجيا", Slug = "tech-ar", Content = "محتوى" },
                        new() { LanguageCode = "de", Title = "Technologie", Slug = "tech-de", Content = "Inhalt" }
                    }
                }
            };

            var filter = new ArticleFilter { Page = 1, PageSize = 10 };

            _mockArticleRepository
                .Setup(r => r.GetAllAsync(filter))
                .ReturnsAsync((articles, 1));

            // Act
            var result = await _service.GetArticlesAsync(filter);

            // Assert
            Assert.Equal("تكنولوجيا", result.Items.First().Title);
            Assert.Equal("ar", result.Items.First().LanguageCode);
        }
        finally
        {
            CultureInfo.CurrentCulture = originalCulture;
        }
    }

    // ---------------------------------------------------------------
    // UpdateArticleAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task UpdateArticleAsync_ArticleNotFound_ReturnsNull()
    {
        // Arrange
        _mockArticleRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Article?)null);

        var dto = new UpdateArticleDto { Id = 999, Translations = new List<ArticleTranslationDto>() };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateArticleAsync_ValidDto_UpdatesCategoryAndImageUrl()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            CategoryId = 1,
            ImageUrl = "old.png",
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Old", Slug = "old", Content = "Old body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            CategoryId = 9,
            ImageUrl = "new.png",
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Old", Slug = "old", Content = "Old body" }
            }
        };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Equal(9, result!.CategoryId);
        Assert.Equal("new.png", result.ImageUrl);
    }

    [Fact]
    public async Task UpdateArticleAsync_ExistingTranslation_UpdatesFieldsInPlace()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Old Title", Slug = "old-slug", Content = "Old body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "New Title", Slug = "new-slug", Content = "New body" }
            }
        };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Single(result!.Translations);
        Assert.Equal("New Title", result.Translations.First().Title);
        Assert.Equal("New body", result.Translations.First().Content);
    }

    [Fact]
    public async Task UpdateArticleAsync_NewLanguageCode_AddsNewTranslation()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" },
                new() { LanguageCode = "ar", Title = "تقنية", Slug = "taqniya", Content = "محتوى" }
            }
        };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Equal(2, result!.Translations.Count);
        Assert.Contains(result.Translations, t => t.LanguageCode == "ar" && t.Title == "تقنية");
    }

    [Fact]
    public async Task UpdateArticleAsync_MissingLanguageCodeInDto_RemovesTranslation()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" },
                new() { LanguageCode = "ar", Title = "تقنية", Slug = "taqniya", Content = "محتوى" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Single(result!.Translations);
        Assert.DoesNotContain(result.Translations, t => t.LanguageCode == "ar");
    }

    [Fact]
    public async Task UpdateArticleAsync_NewTagIds_AddsNewArticleTags()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Tags = new List<ArticleTag> { new() { TagId = 1 } },
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            TagIds = new List<int> { 1, 2 },
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Equal(2, result!.TagIds.Count);
        Assert.Contains(2, result.TagIds);
    }

    [Fact]
    public async Task UpdateArticleAsync_MissingTagIdInDto_RemovesArticleTag()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Tags = new List<ArticleTag> { new() { TagId = 1 }, new() { TagId = 2 } },
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            TagIds = new List<int> { 1 },
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Single(result!.TagIds);
        Assert.DoesNotContain(2, result.TagIds);
    }

    [Fact]
    public async Task UpdateArticleAsync_DuplicateTagIdAlreadyAssigned_DoesNotAddDuplicate()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Tags = new List<ArticleTag> { new() { TagId = 1 } },
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            TagIds = new List<int> { 1 },
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        // Act
        var result = await _service.UpdateArticleAsync(dto);

        // Assert
        Assert.Single(result!.TagIds);
    }

    [Fact]
    public async Task UpdateArticleAsync_ValidDto_CallsRepositoryUpdateAsyncOnce()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var dto = new UpdateArticleDto
        {
            Id = 1,
            Translations = new List<ArticleTranslationDto>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        // Act
        await _service.UpdateArticleAsync(dto);

        // Assert
        _mockArticleRepository.Verify(r => r.UpdateAsync(article), Times.Once);
    }

    // ---------------------------------------------------------------
    // DeleteArticleAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task DeleteArticleAsync_ArticleExists_ReturnsTrueAndCallsDeleteAsync()
    {
        // Arrange
        var article = new Article { Id = 1 };
        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        // Act
        var result = await _service.DeleteArticleAsync(1);

        // Assert
        Assert.True(result);
        _mockArticleRepository.Verify(r => r.DeleteAsync(article), Times.Once);
    }

    [Fact]
    public async Task DeleteArticleAsync_ArticleNotFound_ReturnsFalse()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article?)null);

        // Act
        var result = await _service.DeleteArticleAsync(999);

        // Assert
        Assert.False(result);
        _mockArticleRepository.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
    }

    // ---------------------------------------------------------------
    // PublishArticleAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task PublishArticleAsync_ArticleExists_SetsStatusToPublished()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Status = ArticleStatus.Draft,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        // Act
        var result = await _service.PublishArticleAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ArticleStatus.Published, result!.Status);
        Assert.Equal(ArticleStatus.Published, article.Status);
    }

    [Fact]
    public async Task PublishArticleAsync_ArticleExists_SetsPublishedAtToUtcNow()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Status = ArticleStatus.Draft,
            PublishedAt = default,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        var before = DateTime.UtcNow;

        // Act
        var result = await _service.PublishArticleAsync(1);

        var after = DateTime.UtcNow;

        // Assert
        Assert.True(result!.PublishedAt >= before && result.PublishedAt <= after);
    }

    [Fact]
    public async Task PublishArticleAsync_ArticleNotFound_ReturnsNull()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article?)null);

        // Act
        var result = await _service.PublishArticleAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task PublishArticleAsync_ArticleExists_CallsRepositoryUpdateAsyncOnce()
    {
        // Arrange
        var article = new Article
        {
            Id = 1,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Tech", Slug = "tech", Content = "Body" }
            }
        };

        _mockArticleRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(article);

        // Act
        await _service.PublishArticleAsync(1);

        // Assert
        _mockArticleRepository.Verify(r => r.UpdateAsync(article), Times.Once);
    }

    [Fact]
    public async Task PublishArticleAsync_ArticleNotFound_NeverCallsUpdateAsync()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article?)null);

        // Act
        await _service.PublishArticleAsync(999);

        // Assert
        _mockArticleRepository.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
    }
}