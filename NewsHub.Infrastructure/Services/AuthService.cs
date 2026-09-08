using Microsoft.AspNetCore.Identity;
using NewsHub.Application.Site.DTOs.Auth;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Infrastructure.Identity;

namespace NewsHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailSender _emailSender;
    
    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        var existing  = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null)
            return AuthResult.Failure("Email is already registered.");

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.Phone
        };
        
        var result = await _userManager.CreateAsync(user, dto.Password);
        
        if (!result.Succeeded)
            return AuthResult.Failure(result.Errors.Select(e => e.Description).ToArray());
        
        await _userManager.AddToRoleAsync(user, "User");
        
        // Generate + send email confirmation token (via a separate IEmailSender/INotificationService)
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var confirmLink = $"https://yourdomain.com/Auth/VerifyEmail?userId={user.Id}&token={encodedToken}";

        await _emailSender.SendAsync(user.Email!, "Confirm your NewsHub account",
            $"<p>Welcome {user.FullName}, please confirm your email: <a href='{confirmLink}'>Confirm</a></p>");


        return AuthResult.Success(user.Id.ToString(), user.Email!, user.FullName);
    }

    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || !user.IsActive)
            return AuthResult.Failure("Invalid credentials.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
            return AuthResult.Failure("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);

        return AuthResult.Success(user.Id.ToString(), user.Email!, user.FullName , roles);
    }
    
    public async Task RequestPasswordResetAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) return; // don't reveal whether the email exists

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = Uri.EscapeDataString(token);
        var resetLink = $"https://yourdomain.com/Auth/ResetPassword?email={email}&token={encodedToken}";

        await _emailSender.SendAsync(email, "Reset your NewsHub password",
            $"<p>Click to reset your password: <a href='{resetLink}'>Reset</a></p>");
    }

    public async Task ResetPasswordAsync(string email, string newPassword, string token)
    {
        var user = await _userManager.FindByEmailAsync(email)
                   ?? throw new InvalidOperationException("Invalid reset request.");

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task VerifyEmailAsync(string token)
    {
        // Needs userId alongside the token in practice — see note below
        throw new NotImplementedException();
    }
}