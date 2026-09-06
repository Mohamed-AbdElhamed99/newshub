namespace NewsHub.Application.Site.DTOs.Auth;

public class RegisterDto
{
    public string Email { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;
}