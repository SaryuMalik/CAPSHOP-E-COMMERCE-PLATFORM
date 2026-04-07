using CapShop.AuthService.Application.Commands;
using CapShop.AuthService.Application.DTOs;
using CapShop.AuthService.Application.Interfaces;
using CapShop.AuthService.Domain.Entities;
using CapShop.AuthService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CapShop.AuthService.Tests;

[TestFixture]
public class AuthCommandHandlerTests
{
    private Mock<IUserRepository> _userRepoMock;
    private Mock<IEmailService> _emailServiceMock;
    private IConfiguration _config;
    private AuthCommandHandler _handler;

    [SetUp]
    public void Setup()
    {
        _userRepoMock     = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();

        var configData = new Dictionary<string, string?>
        {
            ["Jwt:Key"]           = "TestSecretKeyMustBe32CharsLong!!",
            ["Jwt:Issuer"]        = "CapShop",
            ["Jwt:Audience"]      = "CapShopUsers",
            ["Jwt:ExpiryMinutes"] = "60"
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _handler = new AuthCommandHandler(_userRepoMock.Object, _config, _emailServiceMock.Object);
    }

    // ── Register Tests ──────────────────────────────────────────────

    [Test]
    public async Task Register_WithNewEmail_ReturnsSuccess()
    {
        // Arrange
        _userRepoMock.Setup(r => r.EmailExistsAsync("new@test.com")).ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                         .Returns(Task.CompletedTask);

        var dto = new RegisterDto
        {
            FirstName = "John",
            LastName  = "Doe",
            Email     = "new@test.com",
            Password  = "password123"
        };

        // Act
        var result = await _handler.RegisterAsync(dto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Message, Is.EqualTo("Registered successfully"));
    }

    [Test]
    public async Task Register_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        _userRepoMock.Setup(r => r.EmailExistsAsync("existing@test.com")).ReturnsAsync(true);

        var dto = new RegisterDto
        {
            FirstName = "Jane",
            LastName  = "Doe",
            Email     = "existing@test.com",
            Password  = "password123"
        };

        // Act
        var result = await _handler.RegisterAsync(dto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Email already registered"));
    }

    [Test]
    public async Task Register_SavesHashedPassword_NotPlainText()
    {
        // Arrange
        User? savedUser = null;
        _userRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                     .Callback<User>(u => savedUser = u)
                     .Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                         .Returns(Task.CompletedTask);

        var dto = new RegisterDto
        {
            FirstName = "Test",
            LastName  = "User",
            Email     = "test@test.com",
            Password  = "plainpassword"
        };

        // Act
        await _handler.RegisterAsync(dto);

        // Assert — password must be hashed, not plain text
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.Password, Is.Not.EqualTo("plainpassword"));
        Assert.That(BCrypt.Net.BCrypt.Verify("plainpassword", savedUser.Password), Is.True);
    }

    [Test]
    public async Task Register_AssignsCustomerRole()
    {
        // Arrange
        User? savedUser = null;
        _userRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
                     .Callback<User>(u => savedUser = u)
                     .Returns(Task.CompletedTask);
        _userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                         .Returns(Task.CompletedTask);

        var dto = new RegisterDto { FirstName = "A", LastName = "B", Email = "a@b.com", Password = "pass123" };

        // Act
        await _handler.RegisterAsync(dto);

        // Assert
        Assert.That(savedUser!.Role, Is.EqualTo("Customer"));
    }

    // ── Login Tests ─────────────────────────────────────────────────

    [Test]
    public async Task Login_WithValidCredentials_ReturnsTokenAndUser()
    {
        // Arrange
        var hashedPw = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User
        {
            Id        = Guid.NewGuid(),
            Email     = "user@test.com",
            Password  = hashedPw,
            FirstName = "John",
            LastName  = "Doe",
            Role      = "Customer"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

        var dto = new LoginDto { Email = "user@test.com", Password = "correctpassword" };

        // Act
        var result = await _handler.LoginAsync(dto);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
        Assert.That(result.User, Is.Not.Null);
    }

    [Test]
    public async Task Login_WithWrongPassword_ReturnsFailure()
    {
        // Arrange
        var hashedPw = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new User
        {
            Id       = Guid.NewGuid(),
            Email    = "user@test.com",
            Password = hashedPw,
            Role     = "Customer"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

        var dto = new LoginDto { Email = "user@test.com", Password = "wrongpassword" };

        // Act
        var result = await _handler.LoginAsync(dto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Invalid email or password"));
    }

    [Test]
    public async Task Login_WithNonExistentEmail_ReturnsFailure()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByEmailAsync("nobody@test.com")).ReturnsAsync((User?)null);

        var dto = new LoginDto { Email = "nobody@test.com", Password = "anypassword" };

        // Act
        var result = await _handler.LoginAsync(dto);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Is.EqualTo("Invalid email or password"));
    }

    [Test]
    public async Task Login_GeneratesValidJwtToken()
    {
        // Arrange
        var hashedPw = BCrypt.Net.BCrypt.HashPassword("pass123");
        var user = new User
        {
            Id        = Guid.NewGuid(),
            Email     = "jwt@test.com",
            Password  = hashedPw,
            FirstName = "JWT",
            LastName  = "User",
            Role      = "Customer"
        };

        _userRepoMock.Setup(r => r.GetByEmailAsync("jwt@test.com")).ReturnsAsync(user);

        var dto = new LoginDto { Email = "jwt@test.com", Password = "pass123" };

        // Act
        var result = await _handler.LoginAsync(dto);

        // Assert — JWT has 3 parts separated by dots
        Assert.That(result.Token, Is.Not.Null);
        Assert.That(result.Token!.Split('.').Length, Is.EqualTo(3));
    }
}
