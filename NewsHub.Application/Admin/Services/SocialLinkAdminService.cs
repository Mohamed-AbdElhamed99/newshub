namespace NewsHub.Application.Admin.Services;

using NewsHub.Application.Admin.DTOs.SocialLinks;
using NewsHub.Application.Admin.Interfaces.Services;
using NewsHub.Application.Admin.Interfaces.Repositories;
using NewsHub.Domain.Entities;

public class SocialLinkAdminService : ISocialLinkAdminService
{
    private readonly ISocialLinkRepository _repository;

    public SocialLinkAdminService(ISocialLinkRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<SocialLinkAdminDto>> GetAllAsync()
    {
        var links = await _repository.GetAllAsync();
        return links.OrderBy(l => l.DisplayOrder).Select(MapToDto).ToList();
    }

    public async Task<SocialLinkAdminDto?> GetByIdAsync(int id)
    {
        var link = await _repository.GetByIdAsync(id);
        return link == null ? null : MapToDto(link);
    }

    public async Task<SocialLinkAdminDto> CreateAsync(SocialLinkUpsertDto dto)
    {
        var link = new SocialLink
        {
            Platform = dto.Platform,
            Url = dto.Url,
            IconClass = dto.IconClass,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(link);
        return MapToDto(link);
    }

    public async Task<SocialLinkAdminDto?> UpdateAsync(SocialLinkUpsertDto dto)
    {
        if (dto.Id == null) return null;

        var link = await _repository.GetByIdAsync(dto.Id.Value);
        if (link == null) return null;

        link.Platform = dto.Platform;
        link.Url = dto.Url;
        link.IconClass = dto.IconClass;
        link.DisplayOrder = dto.DisplayOrder;
        link.IsActive = dto.IsActive;
        link.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(link);
        return MapToDto(link);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var link = await _repository.GetByIdAsync(id);
        if (link == null) return false;

        await _repository.DeleteAsync(link);
        return true;
    }

    private static SocialLinkAdminDto MapToDto(SocialLink link) =>
        new SocialLinkAdminDto
        {
            Id = link.Id,
            Platform = link.Platform,
            Url = link.Url,
            IconClass = link.IconClass,
            DisplayOrder = link.DisplayOrder,
            IsActive = link.IsActive
        };
}