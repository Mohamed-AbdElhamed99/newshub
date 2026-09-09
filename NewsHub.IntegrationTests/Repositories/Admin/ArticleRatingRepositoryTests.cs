using FluentAssertions;
using NewsHub.Application.Admin.DTOs.ArticleRatings;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Identity;
using NewsHub.Infrastructure.Repositories.Admin;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class ArticleRatingRepositoryTests : IDisposable
{
    private readonly ArticleRatingRepositoryFixture _fixture;
    private readonly ArticleRatingRepository _sut;
    private readonly Article _article;

    public ArticleRatingRepositoryTests()
    {
        _fixture = new ArticleRatingRepositoryFixture();
        _sut = new ArticleRatingRepository(_fixture.Context);

        var category = _fixture.SeedCategory();
        var user = _fixture.SeedUser();
        _article = _fixture.SeedArticle(category, user);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task GetByIdAsync_WhenRatingExists_ReturnsRating()
    {
        var siteUser = _fixture.SeedUser();
        var seeded = _fixture.SeedRating(_article, 4 , siteUser.Id);

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Rating.Should().Be(4);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRatingDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllRatingsAndCorrectTotalCount()
    {
        var siteUser1 =  _fixture.SeedUser();
        var siteUser2 =  _fixture.SeedUser();
        _fixture.SeedRating(_article, 5 , siteUser1.Id);
        _fixture.SeedRating(_article, 3 , siteUser2.Id);

        var (items, totalCount) = await _sut.GetAllAsync(new ArticleRatingFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByArticleId_ReturnsOnlyMatchingRatings()
    {
        var otherCategory = _fixture.SeedCategory();
        var otherUser = _fixture.SeedUser();
        var otherArticle = _fixture.SeedArticle(otherCategory, otherUser);

        _fixture.SeedRating(_article, 5 , otherUser.Id);
        _fixture.SeedRating(otherArticle, 2, otherUser.Id);

        var (items, totalCount) = await _sut.GetAllAsync(new ArticleRatingFilter { ArticleId = _article.Id });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(r => r.ArticleId == _article.Id);
    }

    [Fact]
    public async Task GetSummaryByArticleIdAsync_WithRatings_ReturnsCorrectAverageAndCount()
    {
        var siteUser1 =  _fixture.SeedUser();
        var siteUser2 =  _fixture.SeedUser();
        _fixture.SeedRating(_article, 4 , siteUser1.Id);
        _fixture.SeedRating(_article, 2 , siteUser2.Id);

        var (average, totalRatings) = await _sut.GetSummaryByArticleIdAsync(_article.Id);

        totalRatings.Should().Be(2);
        average.Should().Be(3d);
    }

    [Fact]
    public async Task GetSummaryByArticleIdAsync_WithNoRatings_ReturnsZeroes()
    {
        var (average, totalRatings) = await _sut.GetSummaryByArticleIdAsync(_article.Id);

        totalRatings.Should().Be(0);
        average.Should().Be(0d);
    }

    [Fact]
    public async Task DeleteAsync_RemovesRatingFromDatabase()
    {
        var siteUser = _fixture.SeedUser();
        var seeded = _fixture.SeedRating(_article, 5 , siteUser.Id);

        using (var deleteContext = _fixture.CreateAssertionContext())
        {
            var toDelete = await deleteContext.ArticleRatings.FindAsync(seeded.Id);
            var repo = new ArticleRatingRepository(deleteContext);
            await repo.DeleteAsync(toDelete!);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var deleted = await assertContext.ArticleRatings.FindAsync(seeded.Id);

        deleted.Should().BeNull();
    }
}