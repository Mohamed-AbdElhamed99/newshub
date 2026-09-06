using NewsHub.Domain.Enums;

namespace NewsHub.Tests.DomainTests.BusinessRulesTests;

using NewsHub.Domain.Entities;
using NewsHub.Domain.BusinessRules;
using Xunit;

public class ArticlePublishingRulesTests
{
    [Fact]
    public void CanPublish_WhenArabicTranslationMissing_ReturnsFalse()
    {
        var article = new Article
        {
            CategoryId = 1,
            AuthorId = Guid.NewGuid(),
            ImageUrl = "image.jpg",
            Status = ArticleStatus.Submitted,
            Translations = new List<ArticleTranslation>
            {
                new ArticleTranslation { LanguageCode = "en", Title = "Breaking News", Content = "Some text" }
            },
        };
        
        var result = ArticlePublishingRules.CanPublish(article);
        
        Assert.False(result);
    }

    [Fact]
    public void CanPublish_WhenEnglishTranslationMissing_ReturnsFalse()
    {
        var article = new Article
        {
            CategoryId = 1,
            AuthorId = Guid.NewGuid(),
            ImageUrl = "image.jpg",
            Status = ArticleStatus.Submitted,
            Translations = new List<ArticleTranslation>
            {
                new ArticleTranslation { LanguageCode = "ar", Title = "Breaking News", Content = "Some text" }
            },
        };
        
        var result = ArticlePublishingRules.CanPublish(article);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanPublish_WhenCategoryIdMissing_ReturnsFalse()
    {
        var article = new Article
        {
            AuthorId = Guid.NewGuid(),
            ImageUrl = "image.jpg",
            Status = ArticleStatus.Submitted,
            Translations = new List<ArticleTranslation>
            {
                new ArticleTranslation { LanguageCode = "en", Title = "Breaking News", Content = "Some text" },
                new ArticleTranslation { LanguageCode = "ar", Title = "Breaking News", Content = "Some text" },
            },
        };
        
        var result = ArticlePublishingRules.CanPublish(article);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanPublish_WhenAuthorIdMissing_ReturnsFalse()
    {
        var article = new Article
        {
            CategoryId = 1,
            ImageUrl = "image.jpg",
            Status = ArticleStatus.Submitted,
            Translations = new List<ArticleTranslation>
            {
                new ArticleTranslation { LanguageCode = "en", Title = "Breaking News", Content = "Some text" },
                new ArticleTranslation { LanguageCode = "ar", Title = "Breaking News", Content = "Some text" },
            },
        };
        
        var result = ArticlePublishingRules.CanPublish(article);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanPublish_WhenImageUrlMissing_ReturnsFalse()
    {
        var article = new Article
        {
            CategoryId = 1,
            AuthorId = Guid.NewGuid(),
            Status = ArticleStatus.Submitted,
            Translations = new List<ArticleTranslation>
            {
                new ArticleTranslation { LanguageCode = "en", Title = "Breaking News", Content = "Some text" },
                new ArticleTranslation { LanguageCode = "ar", Title = "Breaking News", Content = "Some text" },
            },
        };
        
        var result = ArticlePublishingRules.CanPublish(article);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanPublish_WhenAllRulesAreSatisfied_ReturnsTrue()
    {
        var article = new Article
        {
            CategoryId = 1,
            AuthorId = Guid.NewGuid(),
            ImageUrl = "image.jpg",
            Status = ArticleStatus.Submitted,
            Translations = new List<ArticleTranslation>
            {
                new ArticleTranslation { LanguageCode = "en", Title = "Breaking News", Content = "Some text" },
                new ArticleTranslation { LanguageCode = "ar", Title = "Breaking News", Content = "Some text" },
            },
        };
        
        var result = ArticlePublishingRules.CanPublish(article);
        
        Assert.True(result);
    }
}