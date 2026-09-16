using NewsHub.Application.Site.DTOs.Subscriptions;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _repository;

    public SubscriptionService(ISubscriptionRepository repository)
    {
        _repository = repository;
    }

    public async Task<SubscribeResultDto> SubscribeAsync(string email)
    {
        var existing = await _repository.GetByEmailAsync(email);

        if (existing == null)
        {
            var subscription = new Subscription
            {
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(subscription);
            return new SubscribeResultDto { Success = true };
        }

        if (existing.IsActive)
        {
            // Already subscribed — treat as success, not an error, to avoid leaking whether an email is registered
            return new SubscribeResultDto { Success = true };
        }

        // Previously unsubscribed — reactivate rather than duplicate
        existing.IsActive = true;
        existing.UnsubscribedAt = null;
        await _repository.UpdateAsync(existing);

        return new SubscribeResultDto { Success = true };
    }

    public async Task<UnsubscribeResultDto> UnsubscribeAsync(Guid token)
    {
        var subscription = await _repository.GetByTokenAsync(token);

        if (subscription == null)
        {
            return new UnsubscribeResultDto { Success = false, ErrorMessage = "Invalid or expired unsubscribe link." };
        }

        if (!subscription.IsActive)
        {
            // Already unsubscribed — idempotent success, not an error
            return new UnsubscribeResultDto { Success = true };
        }

        subscription.IsActive = false;
        subscription.UnsubscribedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(subscription);

        return new UnsubscribeResultDto { Success = true };
    }
}