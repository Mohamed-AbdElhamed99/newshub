using NewsHub.Domain.BusinessRules;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.DomainTests.BusinessRulesTests;

public class CommentRulesTests
{
    [Fact]
    public void CanAdd_ReturnFalse_WhenCommentIsNotBelongToArticle()
    {
        var comment = new Comment { Id = 1 , UserId = Guid.NewGuid() ,Content = "This is my comment"};
        
        var result = CommentRules.CanAdd(comment);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanAdd_ReturnFalse_WhenCommentIsNotBelongToUser()
    {
        var comment = new Comment { Id = 1 , ArticleId = 1,Content = "This is my comment"};
        
        var result = CommentRules.CanAdd(comment);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanAdd_ReturnTrue_WhenValidArticleAndUserExists()
    {
        var comment = new Comment { Id = 1 , UserId = Guid.NewGuid() , ArticleId = 1, Content = "This is my comment"};
        
        var result = CommentRules.CanAdd(comment);
        
        Assert.True(result);
    }

    [Fact]
    public void CanModify_ReturnFalse_WhenCommentIsNotBelongToUser()
    {
        var userId = Guid.NewGuid();
        var comment = new Comment { Id = 1 , UserId = Guid.NewGuid(),Content = "This is my comment"};
        
        var result = CommentRules.CanModify(comment, userId);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanModify_ReturnTrue_WhenUserIsOwner()
    {
        var userId = Guid.NewGuid();
        var comment = new Comment { Id = 1, UserId = userId, Content = "This is my comment" };
    
        var result = CommentRules.CanModify(comment, userId);
    
        Assert.True(result);
    }
}