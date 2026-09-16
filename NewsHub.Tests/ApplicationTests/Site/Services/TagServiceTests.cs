namespace NewsHub.Tests.ApplicationTests.Site.Services;

using System.Globalization;
using Moq;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

public class TagServiceTests
{
    private readonly Mock<ITagRepository> _mockTagRepository;
    private readonly TagService _service;

    public TagServiceTests()
    {
        _mockTagRepository = new Mock<ITagRepository>();
        _service = new TagService(_mockTagRepository.Object);
    }

    [Fact]
    public async Task GetTrendingTagsAsync_RepositoryReturnsTags_MapsToDtoCorrectly()
    {
        var tags = new List<Tag>
        {
            new() { Id = 1, Translations = new List<TagTranslation> { new() { LanguageCode = "en", Name = "Elections", Slug = "elections" } } }
        };

        _mockTagRepository.Setup(r => r.GetTrendingTagsAsync(8)).ReturnsAsync(tags);

        var result = await _service.GetTrendingTagsAsync(8);

        var dto = Assert.Single(result);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Elections", dto.Name);
        Assert.Equal("elections", dto.Slug);
    }

    [Fact]
    public async Task GetTrendingTagsAsync_ValidCount_CallsRepositoryWithSameCount()
    {
        _mockTagRepository.Setup(r => r.GetTrendingTagsAsync(It.IsAny<int>())).ReturnsAsync(new List<Tag>());

        await _service.GetTrendingTagsAsync(8);

        _mockTagRepository.Verify(r => r.GetTrendingTagsAsync(8), Times.Once);
    }

    [Fact]
    public async Task GetTrendingTagsAsync_RepositoryReturnsEmptyList_ReturnsEmptyNotNull()
    {
        _mockTagRepository.Setup(r => r.GetTrendingTagsAsync(It.IsAny<int>())).ReturnsAsync(new List<Tag>());

        var result = await _service.GetTrendingTagsAsync(8);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTrendingTagsAsync_MultipleTags_MapsEachCorrectly()
    {
        var tags = new List<Tag>
        {
            new() { Id = 1, Translations = new List<TagTranslation> { new() { LanguageCode = "en", Name = "Elections", Slug = "elections" } } },
            new() { Id = 2, Translations = new List<TagTranslation> { new() { LanguageCode = "en", Name = "Economy", Slug = "economy" } } }
        };

        _mockTagRepository.Setup(r => r.GetTrendingTagsAsync(It.IsAny<int>())).ReturnsAsync(tags);

        var result = await _service.GetTrendingTagsAsync(8);

        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Id == 1 && t.Name == "Elections");
    }

    [Fact]
    public async Task GetTrendingTagsAsync_NoCultureMatch_FallsBackToEnglish()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");
        try
        {
            var tags = new List<Tag>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<TagTranslation>
                    {
                        new() { LanguageCode = "en", Name = "Elections", Slug = "elections" },
                        new() { LanguageCode = "ar", Name = "انتخابات", Slug = "elections-ar" }
                    }
                }
            };

            _mockTagRepository.Setup(r => r.GetTrendingTagsAsync(It.IsAny<int>())).ReturnsAsync(tags);

            var result = await _service.GetTrendingTagsAsync(8);

            Assert.Equal("Elections", result.First().Name);
            Assert.Equal("en", result.First().LanguageCode);
        }
        finally { CultureInfo.CurrentCulture = originalCulture; }
    }

    [Fact]
    public async Task GetTrendingTagsAsync_NoCultureOrEnglishMatch_FallsBackToFirstTranslation()
    {
        var originalCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("fr");
        try
        {
            var tags = new List<Tag>
            {
                new()
                {
                    Id = 1,
                    Translations = new List<TagTranslation>
                    {
                        new() { LanguageCode = "ar", Name = "انتخابات", Slug = "elections-ar" },
                        new() { LanguageCode = "de", Name = "Wahlen", Slug = "elections-de" }
                    }
                }
            };

            _mockTagRepository.Setup(r => r.GetTrendingTagsAsync(It.IsAny<int>())).ReturnsAsync(tags);

            var result = await _service.GetTrendingTagsAsync(8);

            Assert.Equal("انتخابات", result.First().Name);
            Assert.Equal("ar", result.First().LanguageCode);
        }
        finally { CultureInfo.CurrentCulture = originalCulture; }
    }
}