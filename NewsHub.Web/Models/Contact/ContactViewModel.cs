using System.ComponentModel.DataAnnotations;
using NewsHub.Application.Site.DTOs.Layout;

namespace NewsHub.Web.Models.Contact;

public class ContactViewModel
{
    public ContactInfoDto? ContactInfo { get; set; }
    public IEnumerable<SocialLinkDto> SocialLinks { get; set; } = Enumerable.Empty<SocialLinkDto>();
    public ContactFormViewModel Form { get; set; } = new();
}