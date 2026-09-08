using Moq;
using FluentAssertions;
using NewsHub.Application.Site.DTOs.Auth;

namespace NewsHub.IntegrationTests.Auth;

public class AuthServiceTests : IDisposable
{
    private readonly AuthServiceFixture _fixture;

    public AuthServiceTests()
    {
        _fixture = new AuthServiceFixture();
    }
    
    public void Dispose() => _fixture.Dispose();
    
    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUserAndReturnsSuccess()
    {
        var sut = _fixture.CreateAuthService();
        var dto = new RegisterDto { Email = "test1@newshub.com", Password = "Passw0rd", FullName = "Test User" , UserName = "username_test"};

        var result = await sut.RegisterAsync(dto);

        if (!result.Succeeded)
            Console.WriteLine(string.Join("This is the error -> , ", result.Errors));
        
        result.Succeeded.Should().BeTrue();
        result.Email.Should().Be(dto.Email);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsFailure()
    {
        var sut = _fixture.CreateAuthService();
        var dto = new RegisterDto { Email = "dup@newshub.com", Password = "Passw0rd", FullName = "Dup User" , UserName = "username"};
        await sut.RegisterAsync(dto);

        var result = await sut.RegisterAsync(dto);

        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("already registered"));
    }

    [Fact]
    public async Task RegisterAsync_SendsConfirmationEmail()
    {
        var sut = _fixture.CreateAuthService();
        var dto = new RegisterDto { Email = "confirm@newshub.com", Password = "Passw0rd", FullName = "Confirm User" , UserName = "username_test"};

        var result =await sut.RegisterAsync(dto);
        result.Succeeded.Should().BeTrue();
        _fixture.EmailSenderMock.Verify(e =>
            e.SendAsync(dto.Email, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithCorrectCredentials_ReturnsSuccessWithRoles()
    {
        var sut = _fixture.CreateAuthService();
        var dto = new RegisterDto { Email = "login@newshub.com", Password = "Passw0rd", FullName = "Login User" , UserName = "username_test"};
        await sut.RegisterAsync(dto);

        var result = await sut.LoginAsync(new LoginDto { Email = dto.Email, Password = dto.Password });

        result.Succeeded.Should().BeTrue();
        result.Roles.Should().Contain("User");
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFailure()
    {
        var sut = _fixture.CreateAuthService();
        var dto = new RegisterDto { Email = "wrongpass@newshub.com", Password = "Passw0rd", FullName = "Wrong Pass" };
        await sut.RegisterAsync(dto);

        var result = await sut.LoginAsync(new LoginDto { Email = dto.Email, Password = "IncorrectPass1" });

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ForExistingUser_SendsEmail()
    {
        var sut = _fixture.CreateAuthService();
        var dto = new RegisterDto { Email = "reset@newshub.com", Password = "Passw0rd", FullName = "Reset User" , UserName = "username_test"};
        await sut.RegisterAsync(dto);

        await sut.RequestPasswordResetAsync(dto.Email);

        _fixture.EmailSenderMock.Verify(e =>
            e.SendAsync(dto.Email, It.Is<string>(s => s.Contains("Reset")), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_ForNonexistentUser_DoesNotThrowOrSendEmail()
    {
        var sut = _fixture.CreateAuthService();

        var act = async () => await sut.RequestPasswordResetAsync("ghost@newshub.com");

        await act.Should().NotThrowAsync();
        _fixture.EmailSenderMock.Verify(e =>
            e.SendAsync("ghost@newshub.com", It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}