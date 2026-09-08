namespace NewsHub.Application.Site.DTOs.Auth;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];
    public IEnumerable<string> Roles { get; set; } = [];

    public static AuthResult Success(string userId, string email, string fullName , IEnumerable<string> roles = null) =>
        new() { Succeeded = true, UserId = userId, Email = email, FullName = fullName , Roles = roles };

    public static AuthResult Failure(params string[] errors) =>
        new() { Succeeded = false, Errors = errors };
}