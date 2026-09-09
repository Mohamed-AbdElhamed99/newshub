using FluentAssertions;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

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
    public async Task GetAllAsync_ReturnsAllCategoriesWithTranslations()
    {
        _fixture.SeedCategory("Politics");
        _fixture.SeedCategory("Sports");

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(c => c.Translations.Any(t => t.Name == "Politics"));
    }

    [Fact]
    public async Task GetTopNAsync_OrdersByPublishedArticleCountDescending()
    {
        var user = _fixture.SeedUser();

        var popular = _fixture.SeedCategory("Popular");
        _fixture.SeedPublishedArticle(popular, user);
        _fixture.SeedPublishedArticle(popular, user);

        var quiet = _fixture.SeedCategory("Quiet");
        _fixture.SeedPublishedArticle(quiet, user);

        var result = (await _sut.GetTopNAsync(10)).ToList();

        result.First().Id.Should().Be(popular.Id);
        result.Last().Id.Should().Be(quiet.Id);
    }

    [Fact]
    public async Task GetTopNAsync_RespectsCountLimit()
    {
        _fixture.SeedCategory("One");
        _fixture.SeedCategory("Two");
        _fixture.SeedCategory("Three");

        var result = await _sut.GetTopNAsync(2);

        result.Should().HaveCount(2);
    }
}
