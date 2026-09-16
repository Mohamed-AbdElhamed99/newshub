using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Areas.Admin.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
 
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
 
    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
 
    public string? ReturnUrl { get; set; }
}