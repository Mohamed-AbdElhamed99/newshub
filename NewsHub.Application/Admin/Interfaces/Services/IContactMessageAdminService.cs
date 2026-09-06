using NewsHub.Application.Admin.DTOs.ContactMessages;
using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.Interfaces.Services;

public interface IContactMessageAdminService
{
    Task<PagedResult<ContactMessageAdminDto>> GetMessagesAsync(ContactMessageFilter filter);
    Task<ContactMessageAdminDto?> MarkAsReadAsync(int id);
    Task<bool> DeleteMessageAsync(int id);
}