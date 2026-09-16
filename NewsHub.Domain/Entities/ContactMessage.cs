using NewsHub.Domain.Common;

namespace NewsHub.Domain.Entities;

public class ContactMessage : ICreationAuditable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}