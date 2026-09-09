using FluentAssertions;
using NewsHub.Application.Admin.DTOs.Categories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories.Admin;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class CategoryRepositoryTests : IDisposable
{
    private readonly CategoryRepositoryFixture _fixture;
    private readonly CategoryRepository _sut;

    public CategoryRepositoryTests()
    {
        _fixture = new CategoryRepositoryFixture();
        _sut = new CategoryRepository(_fixture.Context);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task AddAsync_ValidCategory_PersistsToDatabase()
    {
        var category = new Category
        {
            Translations = { new CategoryTranslation { LanguageCode = "en", Name = "Politics", Slug = "politics" } }
        };

        await _sut.AddAsync(category);

        using var assertContext = _fixture.CreateAssertionContext();
        var saved = await assertContext.Categories.FindAsync(category.Id);

        saved.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategoryWithTranslations()
    {
        var seeded = _fixture.SeedCategory("Sports");

        var result = await _sut.GetByIdAsync(seeded.Id);

        result.Should().NotBeNull();
        result!.Translations.Should().ContainSingle(t => t.Name == "Sports");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        var result = await _sut.GetByIdAsync(9999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_NoFilters_ReturnsAllCategoriesAndCorrectTotalCount()
    {
        _fixture.SeedCategory("Politics");
        _fixture.SeedCategory("Sports");

        var (items, totalCount) = await _sut.GetAllAsync(new CategoryFilter { Page = 1, PageSize = 10 });

        totalCount.Should().Be(2);
        items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_FilteredByName_ReturnsMatchingCategories()
    {
        _fixture.SeedCategory("Technology");
        _fixture.SeedCategory("Sports");

        var (items, totalCount) = await _sut.GetAllAsync(new CategoryFilter { Name = "tech" });

        totalCount.Should().Be(1);
        items.Should().ContainSingle(c => c.Translations.Any(t => t.Name == "Technology"));
    }

    [Fact]
    public async Task UpdateAsync_ModifiesExistingCategory()
    {
        var seeded = _fixture.SeedCategory("Old Name");

        using (var updateContext = _fixture.CreateAssertionContext())
        {
            var toUpdate = await updateContext.Categories.FindAsync(seeded.Id);
            toUpdate!.ImagePath = "/images/new.png";

            var repo = new CategoryRepository(updateContext);
            await repo.UpdateAsync(toUpdate);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var updated = await assertContext.Categories.FindAsync(seeded.Id);

        updated!.ImagePath.Should().Be("/images/new.png");
    }

    [Fact]
    public async Task DeleteAsync_RemovesCategoryFromDatabase()
    {
        var seeded = _fixture.SeedCategory("To Delete");

        using (var deleteContext = _fixture.CreateAssertionContext())
        {
            var toDelete = await deleteContext.Categories.FindAsync(seeded.Id);
            var repo = new CategoryRepository(deleteContext);
            await repo.DeleteAsync(toDelete!);
        }

        using var assertContext = _fixture.CreateAssertionContext();
        var deleted = await assertContext.Categories.FindAsync(seeded.Id);

        deleted.Should().BeNull();
    }
}