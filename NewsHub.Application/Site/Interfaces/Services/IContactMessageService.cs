using NewsHub.Application.Site.DTOs.ContactMessages;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IContactMessageService
{
    Task SubmitMessageAsync(ContactMessageDto dto);
}