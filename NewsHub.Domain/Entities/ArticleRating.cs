namespace NewsHub.Domain.Entities;

public class ArticleRating
{
    public int Id { get; set; }
    
    public int ArticleId { get; set; }
    
    public string UserId { get; set; } =  string.Empty;
    
    public int Rating { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}