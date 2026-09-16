using NewsHub.Application.Admin.DTOs.ContactMessages;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Services;

public class ContactMessageAdminService : IContactMessageAdminService
{
    private readonly IContactMessageRepository _repository;

    public ContactMessageAdminService(IContactMessageRepository repository)
    {
        this._repository = repository;
    }

    public async Task<PagedResult<ContactMessageAdminDto>> GetMessagesAsync(ContactMessageFilter filter)
    {
        var (messages, totalCount) = await _repository.GetAllAsync(filter);

        var items = messages.Select(MapToDto).ToList();

        return new PagedResult<ContactMessageAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<ContactMessageAdminDto?> MarkAsReadAsync(int id)
    {
        var message = await _repository.GetByIdAsync(id);
        if (message == null) return null;

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await _repository.UpdateAsync(message);
        return MapToDto(message);
    }

    public async Task<bool> DeleteMessageAsync(int id)
    {
        var message = await _repository.GetByIdAsync(id);
        if (message == null) return false;

        await _repository.DeleteAsync(message);
        return true;
    }

    private static ContactMessageAdminDto MapToDto(ContactMessage message) =>
        new ContactMessageAdminDto
        {
            Id = message.Id,
            Name = message.Name,
            Email = message.Email,
            Subject = message.Subject,
            Message = message.Message,
            IsRead = message.IsRead,
            ReadAt = message.ReadAt,
            CreatedAt = message.CreatedAt
        };
}