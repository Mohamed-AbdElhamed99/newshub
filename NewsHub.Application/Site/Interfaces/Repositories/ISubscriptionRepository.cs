using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByEmailAsync(string email);
    Task<Subscription?> GetByTokenAsync(Guid token);
    Task AddAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
}