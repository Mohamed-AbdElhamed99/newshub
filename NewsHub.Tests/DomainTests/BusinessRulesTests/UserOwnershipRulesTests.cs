using NewsHub.Domain.BusinessRules;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.DomainTests.BusinessRulesTests;

public class UserOwnershipRulesTests
{
    [Fact]
    public void ShouldReturnFalse_WhenArticleIsNull()
    {
        Article? article = null;
        var result = UserOwnershipRules.IsUserOwnArticle(article, "user-1");
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnFalse_WhenArticleUserIdIsNull()
    {
        var article = new Article { Id = 1, AuthorId = null! };
        var result = UserOwnershipRules.IsUserOwnArticle(article, "user-1");
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnFalse_WhenArticleUserIdDoesNotMatchCurrentUserId()
    {
        var article = new Article { Id = 1, AuthorId = "user-2" };
        var result = UserOwnershipRules.IsUserOwnArticle(article, "user-1");
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnFalse_WhenCurrentUserIdIsNull()
    {
        var article = new Article { Id = 1, AuthorId = "user-1" };
        var result = UserOwnershipRules.IsUserOwnArticle(article, null!);
        Assert.False(result);
    }

    [Fact]
    public void ShouldReturnTrue_WhenArticleUserIdMatchesCurrentUserId()
    {
        var article = new Article { Id = 1, AuthorId = "user-1" };
        var result = UserOwnershipRules.IsUserOwnArticle(article, "user-1");
        Assert.True(result);
    }
}
