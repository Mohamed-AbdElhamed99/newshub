using NewsHub.Application.Admin.DTOs.Categories;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<(List<Category> Items, int TotalCount)> GetAllAsync(CategoryFilter filter);
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Category category);
}