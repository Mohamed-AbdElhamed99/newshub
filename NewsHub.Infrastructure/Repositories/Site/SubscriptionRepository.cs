using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly NewsHubDbContext _context;

    public SubscriptionRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<Subscription?> GetByEmailAsync(string email)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Subscription?> GetByTokenAsync(Guid token)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(s => s.UnsubscribeToken == token);
    }

    public async Task AddAsync(Subscription subscription)
    {
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Subscription subscription)
    {
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();
    }
}
