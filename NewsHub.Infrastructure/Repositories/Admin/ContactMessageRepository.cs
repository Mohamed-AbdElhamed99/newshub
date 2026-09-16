using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Admin.DTOs.ContactMessages;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Admin;

public class ContactMessageRepository : IContactMessageRepository
{
    private readonly NewsHubDbContext _context;

    public ContactMessageRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task<ContactMessage?> GetByIdAsync(int id)
    {
        return await _context.ContactMessages.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<(List<ContactMessage> Items, int TotalCount)> GetAllAsync(ContactMessageFilter filter)
    {
        var query = _context.ContactMessages.AsQueryable();

        if (filter.IsRead.HasValue)
        {
            query = query.Where(m => m.IsRead == filter.IsRead.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task UpdateAsync(ContactMessage message)
    {
        _context.ContactMessages.Update(message);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ContactMessage message)
    {
        _context.ContactMessages.Remove(message);
        await _context.SaveChangesAsync();
    }
}