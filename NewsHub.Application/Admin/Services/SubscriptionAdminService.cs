using NewsHub.Application.Admin.DTOs.Subscriptions;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Application.Common.Pagination;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Services;

public class SubscriptionAdminService : ISubscriptionAdminService
{
    private readonly ISubscriptionRepository _repository;

    public SubscriptionAdminService(ISubscriptionRepository repository)
    {
        this._repository = repository;
    }

    public async Task<PagedResult<SubscriptionAdminDto>> GetSubscriptionsAsync(SubscriptionFilter filter)
    {
        var (subscriptions, totalCount) = await _repository.GetAllAsync(filter);

        var items = subscriptions.Select(MapToDto).ToList();

        return new PagedResult<SubscriptionAdminDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<SubscriptionAdminDto?> DeactivateSubscriptionAsync(int id)
    {
        var subscription = await _repository.GetByIdAsync(id);
        if (subscription == null) return null;

        subscription.IsActive = false;
        subscription.UnsubscribedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(subscription);
        return MapToDto(subscription);
    }

    public async Task<bool> DeleteSubscriptionAsync(int id)
    {
        var subscription = await _repository.GetByIdAsync(id);
        if (subscription == null) return false;

        await _repository.DeleteAsync(subscription);
        return true;
    }

    private static SubscriptionAdminDto MapToDto(Subscription subscription) =>
        new SubscriptionAdminDto
        {
            Id = subscription.Id,
            Email = subscription.Email,
            IsActive = subscription.IsActive,
            CreatedAt = subscription.CreatedAt,
            UnsubscribedAt = subscription.UnsubscribedAt
        };
}