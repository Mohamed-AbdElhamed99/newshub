using NewsHub.Domain.Entities;

namespace NewsHub.IntegrationTests.Repositories.Admin;

public class ContactMessageRepositoryFixture : SqliteInMemoryFixtureBase
{
    public ContactMessage SeedMessage(bool isRead = false)
    {
        var message = new ContactMessage
        {
            Name = "Test Sender",
            Email = $"{Guid.NewGuid():N}@test.com",
            Subject = "Subject",
            Message = "Body",
            IsRead = isRead
        };
        Context.ContactMessages.Add(message);
        Context.SaveChanges();
        return message;
    }
}