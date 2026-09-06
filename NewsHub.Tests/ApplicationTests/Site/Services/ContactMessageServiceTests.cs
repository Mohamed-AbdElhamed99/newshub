using NewsHub.Application.Site.DTOs.ContactMessages;
using NewsHub.Application.Site.Interfaces.Repositories;
using NewsHub.Application.Site.Services;
using NewsHub.Domain.Entities;

namespace NewsHub.Tests.ApplicationTests.Site.Services;

using Xunit;
using Moq;

using System.Threading.Tasks;
public class ContactMessageServiceTests
{
    [Fact]
    public async Task SubmitMessageAsync_ShouldAddMessage_WhenValidDto()
    {
        // Arrange (Red)
        var repoMock = new Mock<IContactMessageRepository>();
        var service = new ContactMessageService(repoMock.Object);
        var dto = new ContactMessageDto
        {
            Name = "Mohamed",
            Email = "test@example.com",
            Subject = "Hello",
            Message = "This is a test"
        };

        // Act (Green)
        await service.SubmitMessageAsync(dto);

        // Assert (Refactor)
        repoMock.Verify(r => r.AddAsync(It.IsAny<ContactMessage>()), Times.Once);
    }

    [Fact]
    public async Task SubmitMessageAsync_ShouldThrow_WhenEmailMissing()
    {
        var repoMock = new Mock<IContactMessageRepository>();
        var service = new ContactMessageService(repoMock.Object);
        var dto = new ContactMessageDto { Message = "Test" };

        await Assert.ThrowsAsync<ArgumentException>(() => service.SubmitMessageAsync(dto));
    } 
}