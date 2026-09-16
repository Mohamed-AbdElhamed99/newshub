using FluentAssertions;
using NewsHub.Application.Site.DTOs.Articles;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Identity;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class ArticleRepositoryTests : IDisposable
{
    private readonly ArticleRepositoryFixture _fixture;
    private readonly ArticleRepository _sut;
    private readonly Category _category;
    private readonly ApplicationUser _user;

    public ArticleRepositoryTests()
    {
        _fixture = new ArticleRepositoryFixture();
        _sut = new ArticleRepository(_fixture.Context);
        _category = _fixture.SeedCategory();
        _user = _fixture.SeedUser();
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task GetByIdAsync_WhenArticlePublished_ReturnsArticle()
    {
        var seeded = _fixture.SeedArticle(_category, _user, title: "Published One");

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(seeded.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenArticleIsDraft_ReturnsNull()
    {
        var seeded = _fixture.SeedArticle(_category, _user, ArticleStatus.Draft, title: "Draft One");

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_WhenSlugMatchesPublishedArticle_ReturnsArticle()
    {
        _fixture.SeedArticle(_category, _user, title: "My Great Title");

        var result = await _sut.GetBySlugAsync("my-great-title");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_WhenSlugDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetBySlugAsync("no-such-slug");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTrendingAsync_ReturnsOnlyTrendingPublishedArticles()
    {
        var trending = _fixture.SeedArticle(_category, _user, isTrending: true, title: "Trending");
        _fixture.SeedArticle(_category, _user, isTrending: false, title: "Not Trending");
        _fixture.SeedArticle(_category, _user, ArticleStatus.Draft, isTrending: true, title: "Trending Draft");

        var result = (await _sut.GetTrendingAsync(10)).ToList();

        result.Should().ContainSingle(a => a.Id == trending.Id);
    }

    [Fact]
    public async Task GetLatestAsync_ReturnsArticlesOrderedByPublishedAtDescendingWithCommentCount()
    {
        var older = _fixture.SeedArticle(_category, _user, title: "Older", publishedAt: DateTime.UtcNow.AddDays(-2));
        var newer = _fixture.SeedArticle(_category, _user, title: "Newer", publishedAt: DateTime.UtcNow.AddDays(-1));
        var userSite = _fixture.SeedUser();
        
        _fixture.SeedComment(newer, CommentStatus.Approved , userSite.Id);
        _fixture.SeedComment(newer, CommentStatus.Pending, userSite.Id);

        var result = (await _sut.GetLatestAsync(10)).ToList();

        result.First().Article.Id.Should().Be(newer.Id);
        result.First().CommentCount.Should().Be(1);
        result.Last().Article.Id.Should().Be(older.Id);
    }

    [Fact]
    public async Task GetTopStoryAsync_PrefersTrendingOverHighestViewed()
    {
        _fixture.SeedArticle(_category, _user, viewCount: 1000, title: "Most Viewed Not Trending");
        var trending = _fixture.SeedArticle(_category, _user, isTrending: true, viewCount: 1, title: "Trending");

        var result = await _sut.GetTopStoryAsync();

        result.Should().NotBeNull();
        result!.Id.Should().Be(trending.Id);
    }

    [Fact]
    public async Task GetTopStoryAsync_WhenNoneTrending_ReturnsHighestViewed()
    {
        var mostViewed = _fixture.SeedArticle(_category, _user, viewCount: 500, title: "Most Viewed");
        _fixture.SeedArticle(_category, _user, viewCount: 10, title: "Less Viewed");

        var result = await _sut.GetTopStoryAsync();

        result.Should().NotBeNull();
        result!.Id.Should().Be(mostViewed.Id);
    }

    [Fact]
    public async Task GetMostViewedAsync_ReturnsArticlesOrderedByViewCountDescending()
    {
        var low = _fixture.SeedArticle(_category, _user, viewCount: 5, title: "Low");
        var high = _fixture.SeedArticle(_category, _user, viewCount: 50, title: "High");

        var result = (await _sut.GetMostViewedAsync(10)).ToList();

        result.First().Article.Id.Should().Be(high.Id);
        result.Last().Article.Id.Should().Be(low.Id);
    }

    [Fact]
    public async Task GetByCategoryAsync_ReturnsOnlyArticlesInThatCategory()
    {
        var otherCategory = _fixture.SeedCategory();
        var inCategory = _fixture.SeedArticle(_category, _user, title: "In Category");
        _fixture.SeedArticle(otherCategory, _user, title: "In Other Category");

        var result = (await _sut.GetByCategoryAsync(_category.Id, 10)).ToList();

        result.Should().ContainSingle(a => a.Article.Id == inCategory.Id);
    }

    [Fact]
    public async Task GetHighestRatedAsync_ReturnsArticlesOrderedByAverageRatingDescending()
    {
        var siteUser1 = _fixture.SeedUser();
        var siteUser2 = _fixture.SeedUser();
        var siteUser3 = _fixture.SeedUser();
        
        var lowRated = _fixture.SeedArticle(_category, _user, title: "Low Rated");
        
        _fixture.SeedRating(lowRated, 1 , siteUser1.Id);

        var highRated = _fixture.SeedArticle(_category, _user, title: "High Rated");
        _fixture.SeedRating(highRated, 5, siteUser2.Id);
        _fixture.SeedRating(highRated, 4, siteUser3.Id);

        var result = (await _sut.GetHighestRatedAsync(10)).ToList();

        result.First().Article.Id.Should().Be(highRated.Id);
        result.First().AverageRating.Should().Be(4.5);
    }

    [Fact]
    public async Task GetHighestRatedAsync_ArticleWithNoRatings_HasZeroAverage()
    {
        var unrated = _fixture.SeedArticle(_category, _user, title: "Unrated");

        var result = (await _sut.GetHighestRatedAsync(10)).ToList();

        result.Should().ContainSingle(a => a.Article.Id == unrated.Id && a.AverageRating == 0);
    }

    [Fact]
    public async Task GetPagedAsync_FilteredByCategoryId_ReturnsOnlyMatching()
    {
        var otherCategory = _fixture.SeedCategory();
        _fixture.SeedArticle(_category, _user, title: "Mine");
        _fixture.SeedArticle(otherCategory, _user, title: "Other");

        var (items, totalCount) = await _sut.GetPagedAsync(
            new ArticleListFilter { CategoryId = _category.Id, Page = 1, PageSize = 10 });

        totalCount.Should().Be(1);
        items.Should().ContainSingle();
    }

    [Fact]
    public async Task GetPagedAsync_FilteredByTagId_ReturnsOnlyTaggedArticles()
    {
        var tag = _fixture.SeedTag("Featured");
        var tagged = _fixture.SeedArticle(_category, _user, title: "Tagged");
        _fixture.AttachTag(tagged, tag);
        _fixture.SeedArticle(_category, _user, title: "Untagged");

        var (items, totalCount) = await _sut.GetPagedAsync(
            new ArticleListFilter { TagId = tag.Id, Page = 1, PageSize = 10 });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(a => a.Article.Id == tagged.Id);
    }

    [Fact]
    public async Task GetPagedAsync_FilteredBySearchTerm_ReturnsMatchingTitles()
    {
        _fixture.SeedArticle(_category, _user, title: "Elections Update");
        _fixture.SeedArticle(_category, _user, title: "Sports Roundup");

        var (items, totalCount) = await _sut.GetPagedAsync(
            new ArticleListFilter { SearchTerm = "elect", Page = 1, PageSize = 10 });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(a => a.Article.Translations.Any(t => t.Title == "Elections Update"));
    }

    [Fact]
    public async Task GetPagedAsync_ExcludesDraftArticles()
    {
        _fixture.SeedArticle(_category, _user, ArticleStatus.Draft, title: "Draft");
        _fixture.SeedArticle(_category, _user, title: "Published");

        var (_, totalCount) = await _sut.GetPagedAsync(new ArticleListFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithPaging_ReturnsCorrectSlice()
    {
        for (var i = 1; i <= 5; i++)
        {
            _fixture.SeedArticle(_category, _user, title: $"Article {i}", publishedAt: DateTime.UtcNow.AddMinutes(-i));
        }

        var (items, totalCount) = await _sut.GetPagedAsync(new ArticleListFilter { Page = 2, PageSize = 2 });

        totalCount.Should().Be(5);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetApprovedCommentCountAsync_CountsOnlyApprovedComments()
    {
        var article = _fixture.SeedArticle(_category, _user, title: "With Comments");
        var siteUser = _fixture.SeedUser();
        _fixture.SeedComment(article, CommentStatus.Approved , siteUser.Id);
        _fixture.SeedComment(article, CommentStatus.Approved, siteUser.Id);
        _fixture.SeedComment(article, CommentStatus.Pending, siteUser.Id);

        var count = await _sut.GetApprovedCommentCountAsync(article.Id);

        count.Should().Be(2);
    }

    [Fact]
    public async Task IncrementViewCountAsync_IncrementsExistingArticleViewCount()
    {
        var article = _fixture.SeedArticle(_category, _user, viewCount: 10, title: "Views");

        await _sut.IncrementViewCountAsync(article.Id);

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Articles.FindAsync(article.Id);

        updated!.ViewCount.Should().Be(11);
    }

    [Fact]
    public async Task IncrementViewCountAsync_WhenArticleDoesNotExist_DoesNotThrow()
    {
        var act = async () => await _sut.IncrementViewCountAsync(9999);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingArticle()
    {
        var seeded = _fixture.SeedArticle(_category, _user, title: "Original");

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.Articles.FindAsync(seeded.Id);
            toUpdate!.IsTrending = true;

            var repo = new ArticleRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Articles.FindAsync(seeded.Id);

        updated!.IsTrending.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenArticleExists_ReturnsTrue()
    {
        var seeded = _fixture.SeedArticle(_category, _user, title: "Exists");

        var result = await _sut.ExistsAsync(seeded.Id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenArticleDoesNotExist_ReturnsFalse()
    {
        var result = await _sut.ExistsAsync(9999);

        result.Should().BeFalse();
    }
}
