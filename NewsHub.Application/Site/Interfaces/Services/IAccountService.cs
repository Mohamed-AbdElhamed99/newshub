namespace NewsHub.Application.Site.Interfaces.Services;

public interface IAccountService
{
    Task DeactivateAsync(Guid userId);
    Task ReactivateAsync(Guid userId);
}