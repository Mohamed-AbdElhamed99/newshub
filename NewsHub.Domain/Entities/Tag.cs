using NewsHub.Domain.Common;

namespace NewsHub.Domain.Entities;

public class Tag  : ICreationAuditable , IModificationAuditable
{
    public int Id { get; set; }
    public ICollection<TagTranslation> Translations { get; set; } = new List<TagTranslation>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}