using FluentAssertions;
using NewsHub.Domain.Entities;
using NewsHub.Infrastructure.Repositories.Site;

namespace NewsHub.IntegrationTests.Repositories.Site;

public class ContactMessageRepositoryTests : IDisposable
{
    private readonly ContactMessageRepositoryFixture _fixture;
    private readonly ContactMessageRepository _sut;

    public ContactMessageRepositoryTests()
    {
        _fixture = new ContactMessageRepositoryFixture();
        _sut = new ContactMessageRepository(_fixture.Context);
    }

    public void Dispose() => _fixture.Dispose();

    [Fact]
    public async Task AddAsync_ValidMessage_PersistsToDatabase()
    {
        var message = new ContactMessage
        {
            Name = "Jane",
            Email = "jane@test.com",
            Subject = "Hello",
            Message = "Body text"
        };

        await _sut.AddAsync(message);

        using var assertContext = _fixture.CreateAssertionContext();
        var saved = await assertContext.ContactMessages.FindAsync(message.Id);

        saved.Should().NotBeNull();
        saved!.Email.Should().Be("jane@test.com");
    }
}
