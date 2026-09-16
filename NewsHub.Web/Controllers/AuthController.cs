using Microsoft.AspNetCore.Mvc;
using NewsHub.Application.Site.DTOs.Auth;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Web.Models.Auth;

namespace NewsHub.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.RegisterAsync(new RegisterDto
        {
            Email = model.Email,
            UserName = model.UserName,
            Phone = model.Phone,
            Password = model.Password,
            FullName = model.FullName,
            ImageUrl = string.Empty
        });

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }

        // RegisterAsync only creates the account and sends a confirmation email —
        // it does not sign the user in (see AuthService.RegisterAsync).
        TempData["AuthMessage"] = "Account created — check your email to confirm your address before signing in.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(new LoginDto
        {
            Email = model.Email,
            Password = model.Password,
            RememberMe = model.RememberMe
        });

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }
        
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // RequestPasswordResetAsync deliberately no-ops for unknown emails
        // (see the comment in AuthService) — always show the same message.
        await _authService.RequestPasswordResetAsync(model.Email);

        TempData["AuthMessage"] = "If that email is registered, we've sent a link to reset your password.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            return BadRequest("Missing reset link parameters.");

        return View(new ResetPasswordViewModel { Email = email, Token = token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _authService.ResetPasswordAsync(model.Email, model.NewPassword, model.Token);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }

        TempData["AuthMessage"] = "Your password has been reset — sign in with your new password.";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            return BadRequest("Missing verification link parameters.");

        if (!Guid.TryParse(userId, out var parsedUserId))
            return BadRequest("Invalid verification link.");

        var result = await _authService.VerifyEmailAsync(parsedUserId, token);

        ViewBag.Success = result.Succeeded;
        ViewBag.Message = result.Succeeded
            ? null
            : (result.Errors.FirstOrDefault() ?? "Email verification failed.");

        return View();
    }
}