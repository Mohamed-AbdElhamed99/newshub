using System.ComponentModel.DataAnnotations;

namespace NewsHub.Web.Models.Contact;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Please tell us your name.")]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "We need an email to reply to you.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Give your message a subject.")]
    [StringLength(150)]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Write your message.")]
    [StringLength(4000, MinimumLength = 5)]
    public string Message { get; set; } = string.Empty;
}