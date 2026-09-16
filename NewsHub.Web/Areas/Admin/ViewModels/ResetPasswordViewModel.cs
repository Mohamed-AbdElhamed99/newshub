using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Areas.Admin.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    // The password-reset token issued by Identity, carried through the
    // reset-password link as a query parameter and round-tripped via a
    // hidden field on the form.
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}