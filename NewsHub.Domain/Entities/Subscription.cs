using NewsHub.Domain.Common;

namespace NewsHub.Domain.Entities;

public class Subscription  : ICreationAuditable
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public Guid UnsubscribeToken { get; set; } = Guid.NewGuid();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UnsubscribedAt { get; set; }
}