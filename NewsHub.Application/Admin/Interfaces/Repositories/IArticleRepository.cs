using NewsHub.Application.Admin.DTOs.Articles;
using NewsHub.Domain.Entities;

namespace NewsHub.Application.Admin.Interfaces.Repositories;

public interface IArticleRepository
{
    Task AddAsync(Article article);
    Task<Article?> GetByIdAsync(int id);
    Task<(List<Article> Items, int TotalCount)> GetAllAsync(ArticleFilter filter);
    Task UpdateAsync(Article article);
    Task DeleteAsync(Article article);
}