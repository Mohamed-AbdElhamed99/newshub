using NewsHub.Domain.Entities;

namespace NewsHub.Application.Site.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetTopNAsync(int count);
    Task<IEnumerable<Category>> GetAllAsync();
}