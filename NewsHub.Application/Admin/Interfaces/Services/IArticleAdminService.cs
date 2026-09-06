using NewsHub.Application.Admin.DTOs.Articles;
using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.Interfaces.Services;

public interface IArticleAdminService
{
    Task<ArticleAdminDetailDto> CreateArticleAsync(CreateArticleDto dto);
    Task<ArticleAdminDetailDto?> GetArticleByIdAsync(int id);
    Task<PagedResult<ArticleAdminDto>> GetArticlesAsync(ArticleFilter filter);
    Task<ArticleAdminDetailDto?> UpdateArticleAsync(UpdateArticleDto dto);
    Task<bool> DeleteArticleAsync(int id);
    Task<ArticleAdminDetailDto?> PublishArticleAsync(int id);
}