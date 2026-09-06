using NewsHub.Application.Site.DTOs.ContactMessages;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class ContactMessageService : IContactMessageService
{
    private readonly IContactMessageRepository _repository;

    public ContactMessageService(IContactMessageRepository repository)
    {
        _repository = repository;
    }

    public async Task SubmitMessageAsync(ContactMessageDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Message))
            throw new ArgumentException("Email and Message are required");

        var entity = new ContactMessage
        {
            Name = dto.Name, 
            Email = dto.Email,
            Subject = dto.Subject,
            Message = dto.Message
        };
        await _repository.AddAsync(entity);
    }
}
