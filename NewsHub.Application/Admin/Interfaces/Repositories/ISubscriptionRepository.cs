using NewsHub.Application.Admin.DTOs.Subscriptions;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(int id);
    Task<(List<Subscription> Items, int TotalCount)> GetAllAsync(SubscriptionFilter filter);
    Task UpdateAsync(Subscription subscription);
    Task DeleteAsync(Subscription subscription);
}