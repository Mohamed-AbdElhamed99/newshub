namespace NewsHub.Application.Admin.Interfaces.Services;

using NewsHub.Application.Admin.DTOs.SocialLinks;

public interface ISocialLinkAdminService
{
    Task<List<SocialLinkAdminDto>> GetAllAsync();
    Task<SocialLinkAdminDto?> GetByIdAsync(int id);
    Task<SocialLinkAdminDto> CreateAsync(SocialLinkUpsertDto dto);
    Task<SocialLinkAdminDto?> UpdateAsync(SocialLinkUpsertDto dto);
    Task<bool> DeleteAsync(int id);
}