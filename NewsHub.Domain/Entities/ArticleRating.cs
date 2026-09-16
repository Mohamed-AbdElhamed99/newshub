using NewsHub.Domain.Common;

namespace NewsHub.Domain.Entities;

public class ArticleRating  : ICreationAuditable , IModificationAuditable
{
    public int Id { get; set; }
    
    public int ArticleId { get; set; }
    
    public Guid UserId { get; set; }
    
    public int Rating { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}