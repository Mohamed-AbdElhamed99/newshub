using NewsHub.Domain.Common;

namespace NewsHub.Domain.Entities;

public class Category  : ICreationAuditable , IModificationAuditable
{
    public int Id { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public ICollection<CategoryTranslation> Translations { get; set; } = new List<CategoryTranslation>();
}