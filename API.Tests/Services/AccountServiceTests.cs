using System.Security.Cryptography;
using System.Text;
using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Entities;
using API.Messages;
using API.Repositories.Interfaces;
using API.Services;
using API.Services.Interfaces;
using Moq;

namespace API.Tests.Services;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AccountService _accountService;
    private readonly RegisterRequest _registerRequest;
    private readonly LoginRequest _loginRequest;
    private readonly AppUser _user;

    public AccountServiceTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _accountService = new AccountService(_accountRepositoryMock.Object, _tokenServiceMock.Object);

        _registerRequest = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "Password123!"
        };

        _loginRequest = new LoginRequest
        {
            Email = "john@example.com",
            Password = "Password123!"
        };

        _user = new AppUser
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordSalt = new byte[64],
            PasswordHash = new byte[64]
        };

        using var hmac = new HMACSHA512(_user.PasswordSalt);
        _user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password123!"));
    }

    [Fact]
    public async Task RegisterAsync_WhenRegistrationIsValid_ShouldReturnSuccess()
    {
        // Arrange
        string expectedToken = "valid.jwt.token";
        _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<AppUser>()))
            .Returns(new TokenResponse
            {
                Success = true,
                Message = SuccessMessages.TokenCreated,
                Token = expectedToken
            });
        _accountRepositoryMock.Setup(x => x.AddUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new AppUser
            {
                FirstName = _registerRequest.FirstName,
                LastName = _registerRequest.LastName,
                Email = _registerRequest.Email.ToLower(),
                PasswordHash = new byte[64],
                PasswordSalt = new byte[64]
            });

        // Act
        var result = await _accountService.RegisterAsync(_registerRequest);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(SuccessMessages.UserRegistered, result.Message);
        Assert.Equal("john@example.com", result.UserEmail);
        Assert.Equal(expectedToken, result.Token);

        _accountRepositoryMock.Verify(x => x.AddUserAsync(It.Is<AppUser>(u =>
            u.Email == _registerRequest.Email.ToLower() &&
            u.FirstName == _registerRequest.FirstName &&
            u.LastName == _registerRequest.LastName &&
            u.PasswordHash.Length > 0 &&
            u.PasswordSalt.Length > 0
        )), Times.Once);
        _tokenServiceMock.Verify(x => x.CreateToken(It.IsAny<AppUser>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenTokenServiceFails_ShouldReturnEmptyToken()
    {
        // Arrange
        _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<AppUser>()))
            .Returns(new TokenResponse
            {
                Success = false,
                Message = ErrorMessages.TokenAccessError,
                Token = null
            });
        _accountRepositoryMock.Setup(x => x.AddUserAsync(It.IsAny<AppUser>()))
            .ReturnsAsync(new AppUser
            {
                FirstName = _registerRequest.FirstName,
                LastName = _registerRequest.LastName,
                Email = _registerRequest.Email.ToLower(),
                PasswordHash = new byte[64],
                PasswordSalt = new byte[64]
            });

        // Act
        var result = await _accountService.RegisterAsync(_registerRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessages.TokenAccessError, result.Message);
        Assert.Null(result.Token);
    }

    [Fact]
    public async Task RegisterAsync_WhenRepositoryFails_ShouldThrowException()
    {
        // Arrange
        _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<AppUser>()))
            .Returns(new TokenResponse
            {
                Success = true,
                Message = SuccessMessages.TokenCreated,
                Token = "valid.jwt.token"
            });

        _accountRepositoryMock.Setup(x => x.AddUserAsync(It.IsAny<AppUser>()))
            .ThrowsAsync(new Exception("Database failure"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _accountService.RegisterAsync(_registerRequest));
        Assert.Equal("Database failure", ex.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenEmailIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        _accountRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _accountService.LoginAsync(_loginRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessages.InvalidEmail, result.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ShouldReturnFailure()
    {
        // Arrange
        var invalidRequest = new LoginRequest { Email = _user.Email, Password = "wrongpassword" };
        _accountRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);

        // Act
        var result = await _accountService.LoginAsync(invalidRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessages.InvalidPassword, result.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenTokenServiceReturnsNull_ShouldReturnFailure()
    {
        // Arrange
        _accountRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);
        _tokenServiceMock.Setup(x => x.CreateToken(_user))
            .Returns((TokenResponse?)null);

        // Act
        var result = await _accountService.LoginAsync(_loginRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessages.TokenAccessError, result.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenTokenServiceReturnsEmptyToken_ShouldReturnFailure()
    {
        // Arrange
        _accountRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);
        _tokenServiceMock.Setup(x => x.CreateToken(_user))
            .Returns(new TokenResponse
            {
                Success = false,
                Message = ErrorMessages.TokenAccessError,
                Token = ""
            });

        // Act
        var result = await _accountService.LoginAsync(_loginRequest);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessages.TokenAccessError, result.Message);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ShouldReturnSuccess()
    {
        // Arrange
        _accountRepositoryMock.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(_user);
        _tokenServiceMock.Setup(x => x.CreateToken(_user))
            .Returns(new TokenResponse
            {
                Success = true,
                Message = SuccessMessages.UserLoggedIn,
                Token = "valid.jwt.token"
            });

        // Act
        var result = await _accountService.LoginAsync(_loginRequest);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(SuccessMessages.UserLoggedIn, result.Message);
        Assert.Equal(_user.Email, result.UserEmail);
        Assert.Equal("valid.jwt.token", result.Token);
    }

    [Fact]
    public async Task UserExistsAsync_WhenUserExists_ShouldTrimAndLowercaseEmailAndReturnTrue()
    {
        // Arrange
        var email = "  Test@Example.com  ";
        var normalizedEmail = email.Trim().ToLowerInvariant();
        _accountRepositoryMock.Setup(x => x.UserExistsAsync(normalizedEmail))
            .ReturnsAsync(true);

        // Act
        var result = await _accountService.UserExistsAsync(email);

        // Assert
        Assert.True(result);
        Assert.Equal("test@example.com", normalizedEmail);
        _accountRepositoryMock.Verify(x => x.UserExistsAsync(normalizedEmail), Times.Once);
    }

    [Fact]
    public async Task UserExistsAsync_WhenUserDoesNotExist_ShouldTrimAndLowercaseEmailAndReturnFalse()
    {
        // Arrange
        var email = " noUser@example.com";
        var normalizedEmail = email.Trim().ToLowerInvariant();
        _accountRepositoryMock.Setup(x => x.UserExistsAsync(normalizedEmail))
            .ReturnsAsync(false);

        // Act
        var result = await _accountService.UserExistsAsync(email);

        // Assert
        Assert.False(result);
        Assert.Equal("nouser@example.com", normalizedEmail);
        _accountRepositoryMock.Verify(x => x.UserExistsAsync(normalizedEmail), Times.Once);
    }
}
