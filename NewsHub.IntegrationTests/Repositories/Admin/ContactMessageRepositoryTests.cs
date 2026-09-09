using FluentAssertions;
using NewsHub.Application.Admin.DTOs.ContactMessages;
using NewsHub.Infrastructure.Repositories.Admin;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class ContactMessageRepositoryTests : IDisposable
{
    private readonly ContactMessageRepositoryFixture _fixture;
    private readonly ContactMessageRepository _sut;

    public ContactMessageRepositoryTests()
    {
        _fixture = new ContactMessageRepositoryFixture();
        _sut = new ContactMessageRepository(_fixture.Context);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task GetByIdAsync_WhenMessageExists_ReturnsMessage()
    {
        var seeded = _fixture.SeedMessage();

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(seeded.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMessageDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllMessagesAndCorrectTotalCount()
    {
        _fixture.SeedMessage();
        _fixture.SeedMessage();

        var (items, totalCount) = await _sut.GetAllAsync(new ContactMessageFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByIsRead_ReturnsOnlyMatchingMessages()
    {
        _fixture.SeedMessage(isRead: true);
        _fixture.SeedMessage(isRead: false);

        var (items, totalCount) = await _sut.GetAllAsync(new ContactMessageFilter { IsRead = true });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(m => m.IsRead);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingMessage()
    {
        var seeded = _fixture.SeedMessage(isRead: false);

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.ContactMessages.FindAsync(seeded.Id);
            toUpdate!.IsRead = true;
            toUpdate.ReadAt = DateTime.UtcNow;

            var repo = new ContactMessageRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.ContactMessages.FindAsync(seeded.Id);

        updated!.IsRead.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_RemovesMessageFromDatabase()
    {
        var seeded = _fixture.SeedMessage();

        using (var deleteContext = _fixture.CreateAssertionContext())
        {
            var toDelete = await deleteContext.ContactMessages.FindAsync(seeded.Id);
            var repo = new ContactMessageRepository(deleteContext);
            await repo.DeleteAsync(toDelete!);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var deleted = await assertContext.ContactMessages.FindAsync(seeded.Id);

        deleted.Should().BeNull();
    }
}