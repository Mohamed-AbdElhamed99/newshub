using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.Subscriptions;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Admin;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly NewsHubDbContext _context;

    public SubscriptionRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<Subscription?> GetByIdAsync(int id)
    {
        return await _context.Subscriptions.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<(List<Subscription> Items, int TotalCount)> GetAllAsync(SubscriptionFilter filter)
    {
        var query = _context.Subscriptions.AsQueryable();

        if (filter.IsActive.HasValue)
        {
            query = query.Where(s => s.IsActive == filter.IsActive.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task UpdateAsync(Subscription subscription)
    {
        _context.Subscriptions.Update(subscription);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Subscription subscription)
    {
        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
    }
}