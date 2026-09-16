namespace NewsHub.Tests.ApplicationTests.Site.Services;

using System.Globalization;
using Moq;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

public class ArticleServiceTests
{
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly ArticleService _service;

    public ArticleServiceTests()
    {
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockTagRepository = new Mock<ITagRepository>();
        _service = new ArticleService(_mockArticleRepository.Object, _mockTagRepository.Object);
    }
    
    private static Article MakeArticleWithTags(int id, List<int> tagIds)
    {
        return new Article
        {
            Id = id,
            Tags = tagIds.Select(t => new ArticleTag { ArticleId = id, TagId = t }).ToList(),
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = "en", Title = "Title", Content = "Body", Slug = "slug" }
            }
        };
    }

    private static Article MakeArticle(int id = 1, string lang = "en", string title = "Title",
        string? excerpt = "Excerpt", string content = "Content", string slug = "slug",
        string imageUrl = "img.png", int viewCount = 0, int categoryId = 1,
        DateTime? publishedAt = null)
    {
        return new Article
        {
            Id = id,
            CategoryId = categoryId,
            ImageUrl = imageUrl,
            ViewCount = viewCount,
            PublishedAt = publishedAt ?? default,
            Translations = new List<ArticleTranslation>
            {
                new() { LanguageCode = lang, Title = title, Excerpt = excerpt, Content = content, Slug = slug }
            }
        };
    }

    // ---------------------------------------------------------------
    // GetTrendingAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetTrendingAsync_RepositoryReturnsArticles_MapsToTrendingDtoCorrectly()
    {
        var articles = new List<Article> { MakeArticle(1, title: "Tech News", slug: "tech-news") };
        _mockArticleRepository.Setup(r => r.GetTrendingAsync(5)).ReturnsAsync(articles);

        var result = await _service.GetTrendingAsync(5);

        var dto = Assert.Single(result);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Tech News", dto.Title);
        Assert.Equal("tech-news", dto.Slug);
    }

    [Fact]
    public async Task GetTrendingAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        _mockArticleRepository.Setup(r => r.GetTrendingAsync(It.IsAny<int>())).ReturnsAsync(new List<Article>());

        await _service.GetTrendingAsync(8);

        _mockArticleRepository.Verify(r => r.GetTrendingAsync(8), Times.Once);
    }

    [Fact]
    public async Task GetTrendingAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        _mockArticleRepository.Setup(r => r.GetTrendingAsync(It.IsAny<int>())).ReturnsAsync(new List<Article>());

        var result = await _service.GetTrendingAsync(5);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTrendingAsync_NoCultureMatch_FallsBackToEnglish()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");
        try
        {
            var article = MakeArticle(1, title: "Tech", slug: "tech");
            article.Translations.Add(new ArticleTranslation { LanguageCode = "ar", Title = "تكنولوجيا", Slug = "tech-ar", Content = "محتوى" });

            _mockArticleRepository.Setup(r => r.GetTrendingAsync(It.IsAny<int>())).ReturnsAsync(new List<Article> { article });

            var result = await _service.GetTrendingAsync(5);

            Assert.Equal("Tech", result.First().Title);
            Assert.Equal("en", result.First().LanguageCode);
        }
        finally { CultureInfo.CurrentCulture = originalCulture; }
    }

    [Fact]
    public async Task GetTrendingAsync_NoCultureOrEnglishMatch_FallsBackToFirstTranslation()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");
        try
        {
            var article = new Article
            {
                Id = 1,
                Translations = new List<ArticleTranslation>
                {
                    new() { LanguageCode = "ar", Title = "تكنولوجيا", Slug = "tech-ar", Content = "محتوى" },
                    new() { LanguageCode = "de", Title = "Technologie", Slug = "tech-de", Content = "Inhalt" }
                }
            };

            _mockArticleRepository.Setup(r => r.GetTrendingAsync(It.IsAny<int>())).ReturnsAsync(new List<Article> { article });

            var result = await _service.GetTrendingAsync(5);

            Assert.Equal("تكنولوجيا", result.First().Title);
            Assert.Equal("ar", result.First().LanguageCode);
        }
        finally { CultureInfo.CurrentCulture = originalCulture; }
    }

    // ---------------------------------------------------------------
    // GetLatestAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetLatestAsync_RepositoryReturnsResults_MapsToLatestDtoCorrectly()
    {
        var results = new List<ArticleWithCommentCount>
        {
            new() { CommentCount = 5, Article = MakeArticle(1, title: "Tech News", slug: "tech-news", viewCount: 42, publishedAt: new DateTime(2026,1,1)) }
        };
        _mockArticleRepository.Setup(r => r.GetLatestAsync(10)).ReturnsAsync(results);

        var result = await _service.GetLatestAsync(10);

        var dto = Assert.Single(result);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Tech News", dto.Title);
        Assert.Equal(42, dto.ViewCount);
        Assert.Equal(5, dto.CommentCount);
        Assert.Equal(new DateTime(2026, 1, 1), dto.PublishedAt);
    }

    [Fact]
    public async Task GetLatestAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        _mockArticleRepository.Setup(r => r.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<ArticleWithCommentCount>());

        await _service.GetLatestAsync(10);

        _mockArticleRepository.Verify(r => r.GetLatestAsync(10), Times.Once);
    }

    [Fact]
    public async Task GetLatestAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        _mockArticleRepository.Setup(r => r.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(new List<ArticleWithCommentCount>());

        var result = await _service.GetLatestAsync(10);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLatestAsync_ZeroComments_MapsCommentCountAsZero()
    {
        var results = new List<ArticleWithCommentCount> { new() { CommentCount = 0, Article = MakeArticle(1) } };
        _mockArticleRepository.Setup(r => r.GetLatestAsync(It.IsAny<int>())).ReturnsAsync(results);

        var result = await _service.GetLatestAsync(10);

        Assert.Equal(0, result.First().CommentCount);
    }

    // ---------------------------------------------------------------
    // GetTopStoryAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetTopStoryAsync_RepositoryReturnsArticle_MapsToTopStoryDtoCorrectly()
    {
        var article = MakeArticle(1, title: "Biggest Story", slug: "biggest-story", viewCount: 999, imageUrl: "/images/top.png");
        _mockArticleRepository.Setup(r => r.GetTopStoryAsync()).ReturnsAsync(article);

        var result = await _service.GetTopStoryAsync();

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
        Assert.Equal("Biggest Story", result.Title);
        Assert.Equal(999, result.ViewCount);
    }

    [Fact]
    public async Task GetTopStoryAsync_RepositoryReturnsNull_ReturnsNull()
    {
        _mockArticleRepository.Setup(r => r.GetTopStoryAsync()).ReturnsAsync((Article?)null);

        var result = await _service.GetTopStoryAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTopStoryAsync_CallsRepositoryGetTopStoryAsyncOnce()
    {
        _mockArticleRepository.Setup(r => r.GetTopStoryAsync()).ReturnsAsync((Article?)null);

        await _service.GetTopStoryAsync();

        _mockArticleRepository.Verify(r => r.GetTopStoryAsync(), Times.Once);
    }

    // ---------------------------------------------------------------
    // GetMostViewedAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetMostViewedAsync_RepositoryReturnsResults_MapsToLatestDtoCorrectly()
    {
        var results = new List<ArticleWithCommentCount>
        {
            new() { CommentCount = 3, Article = MakeArticle(1, title: "Most Viewed", slug: "most-viewed", viewCount: 500) }
        };
        _mockArticleRepository.Setup(r => r.GetMostViewedAsync(5)).ReturnsAsync(results);

        var result = await _service.GetMostViewedAsync(5);

        var dto = Assert.Single(result);
        Assert.Equal(500, dto.ViewCount);
        Assert.Equal(3, dto.CommentCount);
    }

    [Fact]
    public async Task GetMostViewedAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        _mockArticleRepository.Setup(r => r.GetMostViewedAsync(It.IsAny<int>())).ReturnsAsync(new List<ArticleWithCommentCount>());

        await _service.GetMostViewedAsync(5);

        _mockArticleRepository.Verify(r => r.GetMostViewedAsync(5), Times.Once);
    }

    [Fact]
    public async Task GetMostViewedAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        _mockArticleRepository.Setup(r => r.GetMostViewedAsync(It.IsAny<int>())).ReturnsAsync(new List<ArticleWithCommentCount>());

        var result = await _service.GetMostViewedAsync(5);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // ---------------------------------------------------------------
    // GetPopularAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetPopularAsync_RepositoryReturnsResults_MapsToPopularDtoCorrectly()
    {
        var results = new List<ArticleWithRating>
        {
            new() { AverageRating = 4.5, Article = MakeArticle(1, title: "Highly Rated", slug: "highly-rated") }
        };
        _mockArticleRepository.Setup(r => r.GetHighestRatedAsync(4)).ReturnsAsync(results);

        var result = await _service.GetPopularAsync(4);

        var dto = Assert.Single(result);
        Assert.Equal("Highly Rated", dto.Title);
        Assert.Equal(4.5, dto.AverageRating);
    }

    [Fact]
    public async Task GetPopularAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        _mockArticleRepository.Setup(r => r.GetHighestRatedAsync(It.IsAny<int>())).ReturnsAsync(new List<ArticleWithRating>());

        await _service.GetPopularAsync(4);

        _mockArticleRepository.Verify(r => r.GetHighestRatedAsync(4), Times.Once);
    }

    [Fact]
    public async Task GetPopularAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        _mockArticleRepository.Setup(r => r.GetHighestRatedAsync(It.IsAny<int>())).ReturnsAsync(new List<ArticleWithRating>());

        var result = await _service.GetPopularAsync(4);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    // ---------------------------------------------------------------
    // GetByCategoryAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetByCategoryAsync_RepositoryReturnsResults_ReturnsCorrectCategoryArticlesDto()
    {
        var results = new List<ArticleWithCommentCount>
        {
            new() { CommentCount = 1, Article = MakeArticle(1, title: "Article A", categoryId: 3) },
            new() { CommentCount = 2, Article = MakeArticle(2, title: "Article B", categoryId: 3) }
        };
        _mockArticleRepository.Setup(r => r.GetByCategoryAsync(3, 7)).ReturnsAsync(results);

        var result = await _service.GetByCategoryAsync(3, "Sports", 7);

        Assert.Equal(3, result.CategoryId);
        Assert.Equal("Sports", result.CategoryName);
        Assert.Equal(2, result.Articles.Count);
    }

    [Fact]
    public async Task GetByCategoryAsync_ValidArgs_CallsRepositoryWithSameCategoryIdAndCount()
    {
        _mockArticleRepository.Setup(r => r.GetByCategoryAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<ArticleWithCommentCount>());

        await _service.GetByCategoryAsync(5, "Life Style", 7);

        _mockArticleRepository.Verify(r => r.GetByCategoryAsync(5, 7), Times.Once);
    }

    [Fact]
    public async Task GetByCategoryAsync_RepositoryReturnsEmptyList_ReturnsEmptyArticlesNotNull()
    {
        _mockArticleRepository.Setup(r => r.GetByCategoryAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<ArticleWithCommentCount>());

        var result = await _service.GetByCategoryAsync(5, "Life Style", 7);

        Assert.NotNull(result.Articles);
        Assert.Empty(result.Articles);
    }

    // ---------------------------------------------------------------
    // GetArticleDetailBySlugAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetArticleDetailBySlugAsync_ArticleExists_ReturnsCorrectlyMappedDetailDto()
    {
        var article = MakeArticle(1, title: "Full Article", content: "Full body content", slug: "full-article", categoryId: 2);
        _mockArticleRepository.Setup(r => r.GetBySlugAsync("full-article")).ReturnsAsync(article);

        var result = await _service.GetArticleDetailBySlugAsync("full-article");

        Assert.NotNull(result);
        Assert.Equal("Full Article", result!.Title);
        Assert.Equal("Full body content", result.Content);
        Assert.Equal(2, result.CategoryId);
    }

    [Fact]
    public async Task GetArticleDetailBySlugAsync_ArticleNotFound_ReturnsNull()
    {
        _mockArticleRepository.Setup(r => r.GetBySlugAsync(It.IsAny<string>())).ReturnsAsync((Article?)null);

        var result = await _service.GetArticleDetailBySlugAsync("missing-slug");

        Assert.Null(result);
    }

    // ---------------------------------------------------------------
    // GetPagedAsync
    // ---------------------------------------------------------------

    /*[Fact]
    public async Task GetPagedAsync_RepositoryReturnsData_MapsPagingMetadataCorrectly()
    {
        var articles = new List<Article> { MakeArticle(1, title: "Paged Article") };
        var filter = new ArticleListFilter { Page = 2, PageSize = 5 };

        _mockArticleRepository.Setup(r => r.GetPagedAsync(filter)).ReturnsAsync((articles, 17));

        var result = await _service.GetPagedAsync(filter);

        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetPagedAsync_MappedItems_DefaultCommentCountToZero()
    {
        var articles = new List<Article> { MakeArticle(1) };
        var filter = new ArticleListFilter { Page = 1, PageSize = 10 };
        _mockArticleRepository.Setup(r => r.GetPagedAsync(filter)).ReturnsAsync((articles, 1));

        var result = await _service.GetPagedAsync(filter);

        Assert.Equal(0, result.Items.First().CommentCount);
    }*/
    
    
    // ---------------------------------------------------------------
    // GetArticleDetailBySlugAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetArticleDetailBySlugAsync_ArticleExists_IncludesCommentCount()
    {
        // Arrange
        var article = MakeArticleWithTags(1, new List<int>());
        _mockArticleRepository.Setup(r => r.GetBySlugAsync("slug")).ReturnsAsync(article);
        _mockArticleRepository.Setup(r => r.GetApprovedCommentCountAsync(1)).ReturnsAsync(12);

        // Act
        var result = await _service.GetArticleDetailBySlugAsync("slug");

        // Assert
        Assert.Equal(12, result!.CommentCount);
    }

    [Fact]
    public async Task GetArticleDetailBySlugAsync_ArticleHasTags_ResolvesTagNamesFromTagRepository()
    {
        // Arrange
        var article = MakeArticleWithTags(1, new List<int> { 5, 9 });
        var tags = new List<Tag>
        {
            new() { Id = 5, Translations = new List<TagTranslation> { new() { LanguageCode = "en", Name = "Economy", Slug = "economy" } } },
            new() { Id = 9, Translations = new List<TagTranslation> { new() { LanguageCode = "en", Name = "Politics", Slug = "politics" } } }
        };

        _mockArticleRepository.Setup(r => r.GetBySlugAsync("slug")).ReturnsAsync(article);
        _mockArticleRepository.Setup(r => r.GetApprovedCommentCountAsync(1)).ReturnsAsync(0);
        _mockTagRepository.Setup(r => r.GetByIdsAsync(It.Is<IEnumerable<int>>(ids => ids.Contains(5) && ids.Contains(9)))).ReturnsAsync(tags);

        // Act
        var result = await _service.GetArticleDetailBySlugAsync("slug");

        // Assert
        Assert.Equal(2, result!.TagNames.Count);
        Assert.Contains("Economy", result.TagNames);
        Assert.Contains("Politics", result.TagNames);
    }

    [Fact]
    public async Task GetArticleDetailBySlugAsync_ArticleHasNoTags_DoesNotCallTagRepository()
    {
        // Arrange
        var article = MakeArticleWithTags(1, new List<int>());
        _mockArticleRepository.Setup(r => r.GetBySlugAsync("slug")).ReturnsAsync(article);
        _mockArticleRepository.Setup(r => r.GetApprovedCommentCountAsync(1)).ReturnsAsync(0);

        // Act
        var result = await _service.GetArticleDetailBySlugAsync("slug");

        // Assert
        Assert.Empty(result!.TagNames);
        _mockTagRepository.Verify(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task GetArticleDetailBySlugAsync_ArticleNotFound_ReturnsNullAndSkipsFurtherLookups()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetBySlugAsync(It.IsAny<string>())).ReturnsAsync((Article?)null);

        // Act
        var result = await _service.GetArticleDetailBySlugAsync("missing-slug");

        // Assert
        Assert.Null(result);
        _mockArticleRepository.Verify(r => r.GetApprovedCommentCountAsync(It.IsAny<int>()), Times.Never);
        _mockTagRepository.Verify(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task GetArticleDetailBySlugAsync_ValidSlug_CallsRepositoryWithSameSlug()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetBySlugAsync(It.IsAny<string>())).ReturnsAsync((Article?)null);

        // Act
        await _service.GetArticleDetailBySlugAsync("some-slug");

        // Assert
        _mockArticleRepository.Verify(r => r.GetBySlugAsync("some-slug"), Times.Once);
    }

    // ---------------------------------------------------------------
    // GetPagedAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetPagedAsync_RepositoryReturnsData_MapsPagingMetadataAndCommentCountsCorrectly()
    {
        // Arrange
        var results = new List<ArticleWithCommentCount>
        {
            new() { CommentCount = 4, Article = MakeArticleWithTags(1, new List<int>()) }
        };
        var filter = new ArticleListFilter { Page = 2, PageSize = 5 };

        _mockArticleRepository.Setup(r => r.GetPagedAsync(filter)).ReturnsAsync((results, 17));

        // Act
        var result = await _service.GetPagedAsync(filter);

        // Assert
        Assert.Equal(17, result.TotalCount);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Single(result.Items);
        Assert.Equal(4, result.Items.First().CommentCount);
    }

    [Fact]
    public async Task GetPagedAsync_RepositoryReturnsEmptyList_ReturnsEmptyItemsNotNull()
    {
        // Arrange
        var filter = new ArticleListFilter { Page = 1, PageSize = 10 };
        _mockArticleRepository.Setup(r => r.GetPagedAsync(filter)).ReturnsAsync((new List<ArticleWithCommentCount>(), 0));

        // Act
        var result = await _service.GetPagedAsync(filter);

        // Assert
        Assert.NotNull(result.Items);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetPagedAsync_FilterWithCategoryAndTag_PassesFilterThroughUnchanged()
    {
        // Arrange
        var filter = new ArticleListFilter { Page = 1, PageSize = 10, CategoryId = 3, TagId = 5, SearchTerm = "election" };
        _mockArticleRepository.Setup(r => r.GetPagedAsync(filter)).ReturnsAsync((new List<ArticleWithCommentCount>(), 0));

        // Act
        await _service.GetPagedAsync(filter);

        // Assert
        _mockArticleRepository.Verify(r => r.GetPagedAsync(filter), Times.Once);
    }

    // ---------------------------------------------------------------
    // RegisterViewAsync
    // ---------------------------------------------------------------

    [Fact]
    public async Task RegisterViewAsync_ValidId_CallsRepositoryIncrementViewCountAsyncOnce()
    {
        // Act
        await _service.RegisterViewAsync(7);

        // Assert
        _mockArticleRepository.Verify(r => r.IncrementViewCountAsync(7), Times.Once);
    }
}