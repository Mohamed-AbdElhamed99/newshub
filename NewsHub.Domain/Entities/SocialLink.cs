using NewsHub.Domain.Common;

namespace NewsHub.Domain.Entities;

public class SocialLink  : ICreationAuditable , IModificationAuditable
{
    public int Id { get; set; }

    public string Platform { get; set; } = string.Empty; // "Facebook", "Twitter", "Instagram", etc.
    public string Url { get; set; } = string.Empty;
    public string? IconClass { get; set; } // optional — e.g. "fa-facebook" if you're using icon fonts
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}