using FluentAssertions;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

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
    public async Task GetTrendingTagsAsync_OrdersByArticleUsageCountDescending()
    {
        var category = _fixture.SeedCategory();
        var user = _fixture.SeedUser();

        var popularTag = _fixture.SeedTag("Popular");
        _fixture.AttachTag(_fixture.SeedArticle(category, user), popularTag);
        _fixture.AttachTag(_fixture.SeedArticle(category, user), popularTag);

        var quietTag = _fixture.SeedTag("Quiet");
        _fixture.AttachTag(_fixture.SeedArticle(category, user), quietTag);

        var result = (await _sut.GetTrendingTagsAsync(10)).ToList();

        result.First().Id.Should().Be(popularTag.Id);
        result.Last().Id.Should().Be(quietTag.Id);
    }

    [Fact]
    public async Task GetTrendingTagsAsync_RespectsCountLimit()
    {
        _fixture.SeedTag("One");
        _fixture.SeedTag("Two");
        _fixture.SeedTag("Three");

        var result = await _sut.GetTrendingTagsAsync(2);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdsAsync_ReturnsOnlyMatchingTagsWithTranslations()
    {
        var first = _fixture.SeedTag("Elections");
        var second = _fixture.SeedTag("Sports");
        _fixture.SeedTag("Unrelated");

        var result = (await _sut.GetByIdsAsync(new[] { first.Id, second.Id })).ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(t => t.Translations.Any(tr => tr.Name == "Elections"));
    }
}
