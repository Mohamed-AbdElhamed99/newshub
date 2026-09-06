using NewsHub.Application.Admin.DTOs.Subscriptions;
using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.Interfaces.Services;

public interface ISubscriptionAdminService
{
    Task<PagedResult<SubscriptionAdminDto>> GetSubscriptionsAsync(SubscriptionFilter filter);
    Task<SubscriptionAdminDto?> DeactivateSubscriptionAsync(int id);
    Task<bool> DeleteSubscriptionAsync(int id);
}