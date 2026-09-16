using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.DTOs.Articles;

public class ArticleWithRating
{
    public Article Article { get; set; } = null!;
    public double AverageRating { get; set; }
}