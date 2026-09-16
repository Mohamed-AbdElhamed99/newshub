namespace NewsHub.Application.Admin.DTOs.ContactMessages;

public class ContactMessageAdminDto
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