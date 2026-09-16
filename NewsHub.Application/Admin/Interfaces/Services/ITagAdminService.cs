using NewsHub.Application.Admin.DTOs.Tags;
using NewsHub.Application.Common.Pagination;

namespace NewsHub.Application.Admin.Interfaces.Services;

public interface ITagAdminService
{
    Task<TagAdminDetailDto> CreateTagAsync(CreateTagDto dto);
    Task<TagAdminDetailDto?> GetTagByIdAsync(int id);
    Task<PagedResult<TagAdminDto>> GetTagsAsync(TagFilter filter);
    Task<TagAdminDetailDto?> UpdateTagAsync(UpdateTagDto dto);
    Task<bool> DeleteTagAsync(int id);
}