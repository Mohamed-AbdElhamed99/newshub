using FluentAssertions;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Identity;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class ArticleRatingRepositoryTests : IDisposable
{
    private readonly ArticleRatingRepositoryFixture _fixture;
    private readonly ArticleRatingRepository _sut;
    private readonly Article _article;
    private readonly ApplicationUser _user;

    public ArticleRatingRepositoryTests()
    {
        _fixture = new ArticleRatingRepositoryFixture();
        _sut = new ArticleRatingRepository(_fixture.Context);

        var category = _fixture.SeedCategory();
        _user = _fixture.SeedUser();
        _article = _fixture.SeedArticle(category, _user);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task AddAsync_ValidRating_PersistsToDatabase()
    {
        var rating = new ArticleRating { ArticleId = _article.Id, UserId = _user.Id, Rating = 4 };

        await _sut.AddAsync(rating);

        using var assertContext = _fixture.CreateAssertionContext();
        var saved = await assertContext.ArticleRatings.FindAsync(rating.Id);

        saved.Should().NotBeNull();
        saved!.Rating.Should().Be(4);
    }

    [Fact]
    public async Task GetByArticleAndUserAsync_WhenRatingExists_ReturnsRating()
    {
        var rating = new ArticleRating { ArticleId = _article.Id, UserId = _user.Id, Rating = 5 };
        _fixture.Context.ArticleRatings.Add(rating);
        _fixture.Context.SaveChanges();

        var result = await _sut.GetByArticleAndUserAsync(_article.Id, _user.Id);

        result.Should().NotBeNull();
        result!.Rating.Should().Be(5);
    }

    [Fact]
    public async Task GetByArticleAndUserAsync_WhenNoRatingExists_ReturnsNull()
    {
        var result = await _sut.GetByArticleAndUserAsync(_article.Id, Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingRating()
    {
        var rating = new ArticleRating { ArticleId = _article.Id, UserId = _user.Id, Rating = 2 };
        _fixture.Context.ArticleRatings.Add(rating);
        _fixture.Context.SaveChanges();

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.ArticleRatings.FindAsync(rating.Id);
            toUpdate!.Rating = 5;

            var repo = new ArticleRatingRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.ArticleRatings.FindAsync(rating.Id);

        updated!.Rating.Should().Be(5);
    }
}
