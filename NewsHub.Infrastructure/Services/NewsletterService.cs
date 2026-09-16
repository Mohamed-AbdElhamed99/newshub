using Hangfire;
using Microsoft.EntityFrameworkCore;
using NewsHub.Application.Site.Interfaces.Services;
using NewsHub.Infrastructure.Data;

namespace NewsHub.Infrastructure.Services;

public class NewsletterService : INewsletterService
{
    private readonly NewsHubDbContext _db;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public NewsletterService(NewsHubDbContext db, IBackgroundJobClient backgroundJobClient)
    {
        _db = db;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task NotifySubscribersAsync(int articleId, string title, string slug)
    {
        var emails = await _db.Subscriptions
            .Where(s => s.IsActive) 
            .Select(s => s.Email)
            .ToListAsync();

        foreach (var email in emails)
        {
            _backgroundJobClient.Enqueue<IEmailSender>(sender =>
                sender.SendAsync(
                    email,
                    $"New article: {title}",
                    $"<p>A new article was just published: <a href='https://localhost:5189/article/details?slug={slug}'>{title}</a></p>"));
        }
    }
}