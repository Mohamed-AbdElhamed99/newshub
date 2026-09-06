using NewsHub.Application.Site.DTOs.Auth;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult> LoginAsync(LoginDto dto);
    Task RequestPasswordResetAsync(string email);
    Task ResetPasswordAsync(string email, string newPassword, string token);
    Task VerifyEmailAsync(string token);
}