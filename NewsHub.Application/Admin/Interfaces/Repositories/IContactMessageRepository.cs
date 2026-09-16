using NewsHub.Application.Admin.DTOs.ContactMessages;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Interfaces.Repositories;

public interface IContactMessageRepository
{
    Task<ContactMessage?> GetByIdAsync(int id);
    Task<(List<ContactMessage> Items, int TotalCount)> GetAllAsync(ContactMessageFilter filter);
    Task UpdateAsync(ContactMessage message);
    Task DeleteAsync(ContactMessage message);
}