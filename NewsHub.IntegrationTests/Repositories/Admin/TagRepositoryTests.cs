using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.Tags;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories.Admin;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class TagRepositoryTests : IDisposable
{
    private readonly TagRepositoryFixture _fixture;
    private readonly TagRepository _sut;

    public TagRepositoryTests()
    {
        _fixture = new TagRepositoryFixture();
        _sut = new TagRepository(_fixture.Context);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task AddAsync_ValidTag_PersistsToDatabase()
    {
        var tag = new Tag
        {
            Translations = { new TagTranslation { LanguageCode = "en", Name = "Breaking", Slug = "breaking" } }
        };

        await _sut.AddAsync(tag);

        using var assertContext = _fixture.CreateAssertionContext();
        var saved = await assertContext.Tags.FindAsync(tag.Id);

        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenTagExists_ReturnsTagWithTranslations()
    {
        var seeded = _fixture.SeedTag("Elections");

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Translations.Should().ContainSingle(t => t.Name == "Elections");
    }

    [Fact]
    public async Task GetByIdAsync_WhenTagDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllTagsAndCorrectTotalCount()
    {
        _fixture.SeedTag("Politics");
        _fixture.SeedTag("Sports");

        var (items, totalCount) = await _sut.GetAllAsync(new TagFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByName_ReturnsMatchingTags()
    {
        _fixture.SeedTag("Technology");
        _fixture.SeedTag("Sports");

        var (items, totalCount) = await _sut.GetAllAsync(new TagFilter { Name = "tech" });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(t => t.Translations.Any(tr => tr.Name == "Technology"));
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingTag()
    {
        var seeded = _fixture.SeedTag("Old Tag");

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.Tags.FindAsync(seeded.Id);
            toUpdate!.Translations.Add(new TagTranslation
            {
                TagId = seeded.Id,
                LanguageCode = "ar",
                Name = "اسم جديد",
                Slug = "new-name-ar"
            });

            var repo = new TagRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Tags
            .Include(t => t.Translations)
            .FirstAsync(t => t.Id == seeded.Id);

        updated.Translations.Should().HaveCount(2);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTagFromDatabase()
    {
        var seeded = _fixture.SeedTag("To Delete");

        using (var deleteContext = _fixture.CreateAssertionContext())
        {
            var toDelete = await deleteContext.Tags.FindAsync(seeded.Id);
            var repo = new TagRepository(deleteContext);
            await repo.DeleteAsync(toDelete!);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var deleted = await assertContext.Tags.FindAsync(seeded.Id);

        deleted.Should().BeNull();
    }
}