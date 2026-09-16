using NewsHub.Domain.BusinessRules;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.DomainTests.BusinessRulesTests;

public class ArticleRatingRulesTests
{
    [Fact]
    public void CanRate_ReturnsFalse_WhenRateValueLargerThan5()
    {
        var articleRate = new ArticleRating { Id = 1, ArticleId = 1, UserId = Guid.NewGuid() , Rating = 6};
        
        var result = ArticleRatingRules.CanRate(articleRate , new List<ArticleRating>());
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanRate_ReturnsFalse_WhenRateValueLessThan1()
    {
        var articleRate = new ArticleRating { Id = 1, ArticleId = 1, UserId = Guid.NewGuid() , Rating = 0};
        
        var result = ArticleRatingRules.CanRate(articleRate , new List<ArticleRating>());
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanRate_ReturnsFalse_WhenUserAlreadyRatedArticle()
    {
        var userId = Guid.NewGuid();
        var existingRatings = new List<ArticleRating>
        {
            new ArticleRating { Id = 1, ArticleId = 1, UserId = userId , Rating = 4}
        };
        var newRating = new  ArticleRating { Id = 1, ArticleId = 1, UserId =userId , Rating = 5};
        
        var result = ArticleRatingRules.CanRate(newRating , existingRatings);
        
        Assert.False(result);
    }

    [Fact]
    public void CanRate_WhenValid_ReturnsTrue()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var existingRatings = new List<ArticleRating>()
        {
            new ArticleRating { Id = 1, ArticleId = 1, UserId = userId1 , Rating = 4}
        };
        var newRating = new ArticleRating { ArticleId = 1, UserId = userId2, Rating = 5 };

        Assert.True(ArticleRatingRules.CanRate(newRating, existingRatings));
    }
}