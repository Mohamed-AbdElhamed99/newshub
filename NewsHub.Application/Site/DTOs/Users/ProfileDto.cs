namespace NewsHub.Application.Site.DTOs.Users;

public class ProfileDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string Bio { get; set; } = default!;
    public string OtherInfo { get; set; } = default!;
    public string? ImageUrl { get; set; }
}