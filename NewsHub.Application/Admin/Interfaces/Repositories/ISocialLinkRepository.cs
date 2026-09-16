namespace NewsHub.Application.Admin.Interfaces.Repositories;

using NewsHub.Domain.Entities;

public interface ISocialLinkRepository
{
    Task<List<SocialLink>> GetAllAsync();
    Task<SocialLink?> GetByIdAsync(int id);
    Task AddAsync(SocialLink link);
    Task UpdateAsync(SocialLink link);
    Task DeleteAsync(SocialLink link);
}