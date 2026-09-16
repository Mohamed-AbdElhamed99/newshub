namespace NewsHub.Application.Admin.Interfaces.Repositories;

using NewsHub.Domain.Entities;

public interface ISiteSettingRepository
{
    Task<SiteSetting?> GetAsync();
    Task UpdateAsync(SiteSetting settings);
}