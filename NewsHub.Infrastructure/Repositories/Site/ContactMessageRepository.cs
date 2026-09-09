using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Repositories.Site;

public class ContactMessageRepository : IContactMessageRepository
{
    private readonly NewsHubDbContext _context;

    public ContactMessageRepository(NewsHubDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ContactMessage entity)
    {
        await _context.ContactMessages.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}
