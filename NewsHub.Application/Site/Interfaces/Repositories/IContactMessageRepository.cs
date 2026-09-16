using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface IContactMessageRepository
{
    Task AddAsync(ContactMessage entity);
}