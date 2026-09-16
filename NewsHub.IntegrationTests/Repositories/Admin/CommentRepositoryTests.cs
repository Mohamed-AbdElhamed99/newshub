using FluentAssertions;
using NewsHub.Application.Admin.DTOs.Comments;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Identity;
using NewsHub.Infrastructure.Repositories.Admin;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class CommentRepositoryTests : IDisposable
{
    private readonly CommentRepositoryFixture _fixture;
    private readonly CommentRepository _sut;
    private readonly Article _article;
    private readonly ApplicationUser otherUser;
    public CommentRepositoryTests()
    {
        _fixture = new CommentRepositoryFixture();
        _sut = new CommentRepository(_fixture.Context);

        var category = _fixture.SeedCategory();
        var user = _fixture.SeedUser();
        otherUser = _fixture.SeedUser();
        _article = _fixture.SeedArticle(category, user);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsComment()
    {
        var seeded = _fixture.SeedComment(article:_article , userId:otherUser.Id);

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.ArticleId.Should().Be(_article.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllCommentsAndCorrectTotalCount()
    {
        var siteUser = _fixture.SeedUser();
        _fixture.SeedComment(article:_article , userId: otherUser.Id);
        _fixture.SeedComment(article:_article ,  userId: siteUser.Id);

        var (items, totalCount) = await _sut.GetAllAsync(new CommentFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByArticleId_ReturnsOnlyMatchingComments()
    {
        var otherCategory = _fixture.SeedCategory();
        var otherUser = _fixture.SeedUser();
        var otherArticle = _fixture.SeedArticle(otherCategory, otherUser);

        _fixture.SeedComment(article:_article  , userId: otherUser.Id);
        _fixture.SeedComment(article:otherArticle, userId: otherUser.Id);

        var (items, totalCount) = await _sut.GetAllAsync(new CommentFilter { ArticleId = _article.Id });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(c => c.ArticleId == _article.Id);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByStatus_ReturnsOnlyMatchingComments()
    {
        _fixture.SeedComment(_article, CommentStatus.Pending , otherUser.Id);
        _fixture.SeedComment(_article, CommentStatus.Approved , otherUser.Id);

        var (items, totalCount) = await _sut.GetAllAsync(new CommentFilter { Status = CommentStatus.Approved });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(c => c.Status == CommentStatus.Approved);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingComment()
    {
        var seeded = _fixture.SeedComment(_article, CommentStatus.Pending , otherUser.Id);

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.Comments.FindAsync(seeded.Id);
            toUpdate!.Status = CommentStatus.Approved;

            var repo = new CommentRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Comments.FindAsync(seeded.Id);

        updated!.Status.Should().Be(CommentStatus.Approved);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCommentFromDatabase()
    {
        var seeded = _fixture.SeedComment(article:_article ,userId : otherUser.Id);

        using (var deleteContext = _fixture.CreateAssertionContext())
        {
            var toDelete = await deleteContext.Comments.FindAsync(seeded.Id);
            var repo = new CommentRepository(deleteContext);
            await repo.DeleteAsync(toDelete!);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var deleted = await assertContext.Comments.FindAsync(seeded.Id);

        deleted.Should().BeNull();
    }
}