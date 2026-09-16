using FluentAssertions;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class SiteSettingsRepositoryTests : IDisposable
{
    private readonly SiteSettingsRepositoryFixture _fixture;
    private readonly SiteSettingsRepository _sut;

    public SiteSettingsRepositoryTests()
    {
        _fixture = new SiteSettingsRepositoryFixture();
        _sut = new SiteSettingsRepository(_fixture.Context);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task GetContactInfoAsync_WhenSettingsExist_ReturnsMappedDto()
    {
        _fixture.SeedSettings("info@newshub.com", "+201000000000", "Cairo, Egypt");

        var result = await _sut.GetContactInfoAsync();

        result.Email.Should().Be("info@newshub.com");
        result.Phone.Should().Be("+201000000000");
        result.Address.Should().Be("Cairo, Egypt");
    }

    [Fact]
    public async Task GetContactInfoAsync_WhenNoSettingsExist_ReturnsEmptyDto()
    {
        var result = await _sut.GetContactInfoAsync();

        result.Email.Should().BeEmpty();
        result.Phone.Should().BeEmpty();
        result.Address.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSocialLinksAsync_ReturnsOnlyActiveLinksOrderedByDisplayOrder()
    {
        _fixture.SeedSocialLink("Twitter", "https://twitter.com/newshub", 2);
        _fixture.SeedSocialLink("Facebook", "https://facebook.com/newshub", 1);
        _fixture.SeedSocialLink("Instagram", "https://instagram.com/newshub", 3, isActive: false);

        var result = (await _sut.GetSocialLinksAsync()).ToList();

        result.Should().HaveCount(2);
        result[0].Platform.Should().Be("Facebook");
        result[1].Platform.Should().Be("Twitter");
    }
}
