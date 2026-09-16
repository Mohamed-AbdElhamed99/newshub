using NewsHub.Application.Site.DTOs.Subscriptions;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface ISubscriptionService
{
    Task<SubscribeResultDto> SubscribeAsync(string email);
    Task<UnsubscribeResultDto> UnsubscribeAsync(Guid token);
}