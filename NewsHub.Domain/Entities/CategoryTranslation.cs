namespace NewsHub.Domain.Entities;

public class CategoryTranslation
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
}