namespace NewsHub.Application.Site.Interfaces.Services;

public interface INewsletterService
{
    Task NotifySubscribersAsync(int articleId, string title, string slug);
}