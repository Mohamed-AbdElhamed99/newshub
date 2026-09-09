using FluentAssertions;
using NewsHub.Application.Admin.DTOs.Articles;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Identity;
using NewsHub.Infrastructure.Repositories.Admin;

namespace NewsHub.IntegrationTests.Repositories.Admin;

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
    public async Task AddAsync_ValidArticle_PersistsToDatabase()
    {
        var article = new Article
        {
            AuthorId = _user.Id,
            CategoryId = _category.Id,
            Status = ArticleStatus.Draft
        };

        await _sut.AddAsync(article);

        using var assertContext = _fixture.CreateAssertionContext();
        var saved = await assertContext.Articles.FindAsync(article.Id);

        saved.Should().NotBeNull();
        saved!.CategoryId.Should().Be(_category.Id);
        saved.AuthorId.Should().Be(_user.Id);
        saved.Status.Should().Be(ArticleStatus.Draft);
    }

    [Fact]
    public async Task GetByIdAsync_WhenArticleExists_ReturnsArticleWithTranslations()
    {
        var seeded = _fixture.SeedArticle(_category, _user, translationTitle: "Breaking News");

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(seeded.Id);
        result.Translations.Should().ContainSingle(t => t.Title == "Breaking News");
    }

    [Fact]
    public async Task GetByIdAsync_WhenArticleDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllArticlesAndCorrectTotalCount()
    {
        _fixture.SeedArticle(_category, _user, translationTitle: "Article One");
        _fixture.SeedArticle(_category, _user, translationTitle: "Article Two");
        _fixture.SeedArticle(_category, _user, translationTitle: "Article Three");

        var (items, totalCount) = await _sut.GetAllAsync(new ArticleFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(3);
        items.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByCategoryId_ReturnsOnlyMatchingArticles()
    {
        var otherCategory = _fixture.SeedCategory();
        _fixture.SeedArticle(_category, _user, translationTitle: "In Category");
        _fixture.SeedArticle(otherCategory, _user, translationTitle: "In Other Category");

        var (items, totalCount) = await _sut.GetAllAsync(new ArticleFilter { CategoryId = _category.Id });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(a => a.CategoryId == _category.Id);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByStatus_ReturnsOnlyMatchingArticles()
    {
        _fixture.SeedArticle(_category, _user, ArticleStatus.Draft, "Draft Article");
        _fixture.SeedArticle(_category, _user, ArticleStatus.Published, "Published Article");

        var (items, totalCount) = await _sut.GetAllAsync(new ArticleFilter { Status = ArticleStatus.Published });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(a => a.Status == ArticleStatus.Published);
    }

    [Fact]
    public async Task GetAllAsync_FilteredBySearchTerm_ReturnsArticlesWithMatchingTranslationTitle()
    {
        _fixture.SeedArticle(_category, _user, translationTitle: "Elections Update");
        _fixture.SeedArticle(_category, _user, translationTitle: "Sports Roundup");

        var (items, totalCount) = await _sut.GetAllAsync(new ArticleFilter { SearchTerm = "elect" });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(a => a.Translations.Any(t => t.Title == "Elections Update"));
    }

    [Fact]
    public async Task GetAllAsync_WithPaging_ReturnsCorrectPageSlice()
    {
        for (var i = 1; i <= 5; i++)
        {
            _fixture.SeedArticle(_category, _user, translationTitle: $"Article {i}");
            await Task.Delay(5); // ensure distinct, increasing CreatedAt for stable ordering
        }

        var (items, totalCount) = await _sut.GetAllAsync(new ArticleFilter { Page = 2, PageSize = 2 });

        totalCount.Should().Be(5);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_OrdersByCreatedAtDescending()
    {
        var first = _fixture.SeedArticle(_category, _user, translationTitle: "First");
        await Task.Delay(5);
        var second = _fixture.SeedArticle(_category, _user, translationTitle: "Second");

        var (items, _) = await _sut.GetAllAsync(new ArticleFilter { Page = 1, PageSize = 10 });

        items.First().Id.Should().Be(second.Id);
        items.Last().Id.Should().Be(first.Id);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingArticle()
    {
        var seeded = _fixture.SeedArticle(_category, _user, translationTitle: "Original");

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.Articles.FindAsync(seeded.Id);
            toUpdate!.Status = ArticleStatus.Published;
            toUpdate.IsTrending = true;

            var repo = new ArticleRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Articles.FindAsync(seeded.Id);

        updated!.Status.Should().Be(ArticleStatus.Published);
        updated.IsTrending.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_RemovesArticleFromDatabase()
    {
        var seeded = _fixture.SeedArticle(_category, _user, translationTitle: "To Delete");

        using (var deleteContext = _fixture.CreateAssertionContext())
        {
            var toDelete = await deleteContext.Articles.FindAsync(seeded.Id);
            var repo = new ArticleRepository(deleteContext);
            await repo.DeleteAsync(toDelete!);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var deleted = await assertContext.Articles.FindAsync(seeded.Id);

        deleted.Should().BeNull();
    }
}