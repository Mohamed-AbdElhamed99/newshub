namespace NewsHub.Application.Site.Interfaces.Services;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}