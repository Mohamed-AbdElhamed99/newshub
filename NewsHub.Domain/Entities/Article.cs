using NewsHub.Domain.Common;
using NewsHub.Domain.Enums;

namespace NewsHub.Domain.Entities;

public class Article : ICreationAuditable , IModificationAuditable
{
    public int Id { get; set; }
    
    public Guid AuthorId { get; set; }
    
    public int CategoryId { get; set; }

    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
    
    public bool IsTrending { get; set; } = false;

    public int ViewCount { get; set; } = 0;
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; } 
    
    public DateTime PublishedAt { get; set; }
    
    public ICollection<ArticleTag> Tags { get; set; } = new List<ArticleTag>();
    
    public ICollection<ArticleTranslation> Translations { get; set; } = new List<ArticleTranslation>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ArticleRating> Ratings { get; set; } = new List<ArticleRating>();

    public void MarkAsPublished()
    {
        Status = ArticleStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }
}