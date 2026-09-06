using NewsHub.Application.Site.DTOs.Auth;
using NewsHub.Application.Site.DTOs.Users;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IUserService
{
    Task<UserDto> RegisterAsync(RegisterDto dto);
    Task<AuthResult> LoginAsync(LoginDto dto);
}
