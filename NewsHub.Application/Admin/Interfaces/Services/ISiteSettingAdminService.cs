namespace NewsHub.Application.Admin.Interfaces.Services;

using NewsHub.Application.Admin.DTOs.SiteSettings;

public interface ISiteSettingAdminService
{
    Task<SiteSettingAdminDto> GetAsync();
    Task<SiteSettingAdminDto> UpdateAsync(UpdateSiteSettingDto dto);
}