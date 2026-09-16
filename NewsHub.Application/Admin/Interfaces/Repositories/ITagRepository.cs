using NewsHub.Application.Admin.DTOs.Tags;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Interfaces.Repositories;

public interface ITagRepository
{
    Task AddAsync(Tag tag);
    Task<Tag?> GetByIdAsync(int id);
    Task<(List<Tag> Items, int TotalCount)> GetAllAsync(TagFilter filter);
    Task UpdateAsync(Tag tag);
    Task DeleteAsync(Tag tag);
}