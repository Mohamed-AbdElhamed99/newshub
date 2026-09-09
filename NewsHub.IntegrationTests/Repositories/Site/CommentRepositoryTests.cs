using FluentAssertions;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Enums;
using NewsHub.Infrastructure.Identity;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class CommentRepositoryTests : IDisposable
{
    private readonly CommentRepositoryFixture _fixture;
    private readonly CommentRepository _sut;
    private readonly Article _article;
    private readonly ApplicationUser _user;

    public CommentRepositoryTests()
    {
        _fixture = new CommentRepositoryFixture();
        _sut = new CommentRepository(_fixture.Context);

        var category = _fixture.SeedCategory();
        _user = _fixture.SeedUser();
        _article = _fixture.SeedArticle(category, _user);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task GetApprovedCommentCountsAsync_ReturnsCountsGroupedByArticle_ApprovedOnly()
    {
        var otherCategory = _fixture.SeedCategory();
        var otherArticle = _fixture.SeedArticle(otherCategory, _user);

        _fixture.SeedComment(_article, _user.Id, CommentStatus.Approved);
        _fixture.SeedComment(_article, _user.Id, CommentStatus.Approved);
        _fixture.SeedComment(_article, _user.Id, CommentStatus.Pending);
        _fixture.SeedComment(otherArticle, _user.Id, CommentStatus.Approved);

        var result = await _sut.GetApprovedCommentCountsAsync(new[] { _article.Id, otherArticle.Id });

        result[_article.Id].Should().Be(2);
        result[otherArticle.Id].Should().Be(1);
    }

    [Fact]
    public async Task AddAsync_ValidComment_PersistsToDatabase()
    {
        var comment = new Comment { ArticleId = _article.Id, UserId = _user.Id, Content = "Hello" };

        await _sut.AddAsync(comment);

        using var assertContext = _fixture.CreateAssertionContext();
        var saved = await assertContext.Comments.FindAsync(comment.Id);

        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsComment()
    {
        var seeded = _fixture.SeedComment(_article, _user.Id);

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByArticleAndUserAsync_ReturnsOnlyThatUsersCommentsOnThatArticle()
    {
        var otherUser = _fixture.SeedUser();
        _fixture.SeedComment(_article, _user.Id);
        _fixture.SeedComment(_article, otherUser.Id);

        var result = (await _sut.GetByArticleAndUserAsync(_article.Id, _user.Id)).ToList();

        result.Should().ContainSingle();
        result[0].UserId.Should().Be(_user.Id);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingComment()
    {
        var seeded = _fixture.SeedComment(_article, _user.Id);

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.Comments.FindAsync(seeded.Id);
            toUpdate!.Content = "Edited";

            var repo = new CommentRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Comments.FindAsync(seeded.Id);

        updated!.Content.Should().Be("Edited");
    }

    [Fact]
    public async Task DeleteAsync_RemovesCommentFromDatabase()
    {
        var seeded = _fixture.SeedComment(_article, _user.Id);

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
