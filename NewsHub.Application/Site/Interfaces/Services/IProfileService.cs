using NewsHub.Application.Site.DTOs.Users;

namespace NewsHub.Application.Site.Interfaces.Services;

public interface IProfileService
{
    Task<ProfileDto> GetProfileAsync(Guid userId);
    Task UpdateProfileAsync(Guid userId, ProfileDto dto);
    Task UpdateProfileImageAsync(Guid userId, byte[] imageBytes);
}