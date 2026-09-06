namespace NewsHub.Domain.Entities;

public class Subscription
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UnsubscribedAt { get; set; }
}