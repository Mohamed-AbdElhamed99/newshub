namespace NewsHub.Application.Admin.Services;

using NewsHub.Application.Admin.DTOs.SiteSettings;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;

public class SiteSettingAdminService : ISiteSettingAdminService
{
    private readonly ISiteSettingRepository _repository;

    public SiteSettingAdminService(ISiteSettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<SiteSettingAdminDto> GetAsync()
    {
        var settings = await _repository.GetAsync() ?? new SiteSetting();
        return MapToDto(settings);
    }

    public async Task<SiteSettingAdminDto> UpdateAsync(UpdateSiteSettingDto dto)
    {
        var settings = await _repository.GetAsync() ?? new SiteSetting();

        settings.ContactEmail = dto.ContactEmail;
        settings.ContactPhone = dto.ContactPhone;
        settings.ContactAddress = dto.ContactAddress;
        settings.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(settings);

        return MapToDto(settings);
    }

    private static SiteSettingAdminDto MapToDto(SiteSetting settings) =>
        new SiteSettingAdminDto
        {
            Id = settings.Id,
            ContactEmail = settings.ContactEmail,
            ContactPhone = settings.ContactPhone,
            ContactAddress = settings.ContactAddress,
            UpdatedAt = settings.UpdatedAt
        };
}