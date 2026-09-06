using NewsHub.Domain.BusinessRules;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.DomainTests.BusinessRulesTests;

public class UserOwnershipRulesTests
{
    [Fact]
    public void ShouldReturnFalse_WhenArticleIsNull()
    {
        Article? article = null;
        var result = UserOwnershipRules.IsUserOwnArticle(article, Guid.NewGuid());
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnFalse_WhenArticleUserIdIsNull()
    {
        var article = new Article { Id = 1};
        var result = UserOwnershipRules.IsUserOwnArticle(article, Guid.NewGuid());
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnFalse_WhenArticleUserIdDoesNotMatchCurrentUserId()
    {
        var article = new Article { Id = 1, AuthorId = Guid.NewGuid() };
        var result = UserOwnershipRules.IsUserOwnArticle(article, Guid.NewGuid());
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnFalse_WhenCurrentUserIdIsNull()
    {
        var article = new Article { Id = 1, AuthorId = Guid.NewGuid() };
        var result = UserOwnershipRules.IsUserOwnArticle(article, Guid.NewGuid());
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnTrue_WhenArticleUserIdMatchesCurrentUserId()
    {
        var userId = Guid.NewGuid();
        var article = new Article { Id = 1, AuthorId =userId };
        var result = UserOwnershipRules.IsUserOwnArticle(article, userId);
        Assert.True(result);
    }
}
