using Microsoft.AspNetCore.Http;
using NewsHub.Application.Site.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using NewsHub.Domain.Exceptions;
using NewsHub.Infrastructure.Identity;

namespace NewsHub.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(UserManager<ApplicationUser> userManager ,IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<bool> IsExistsAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        return user != null;
    }

    public async Task<Guid> GetCurrentUserAsync()   
    {
        var principal = _httpContextAccessor.HttpContext?.User;
        if (principal?.Identity?.IsAuthenticated != true) throw new NotFoundException("No user logged in");
        var user = await _userManager.GetUserAsync(principal);

        var userID = user.Id;
        return userID;
    }
}