using NewsHub.Domain.Common;

namespace NewsHub.Domain.Entities;

public class SiteSetting : IModificationAuditable
{
    public int Id { get; set; }

    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string ContactAddress { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; }
}