using API.Entities;
using API.Messages;
using API.Services;
using API.Settings;
using Microsoft.Extensions.Options;
using Moq;

namespace API.Tests.Services;

public class TokenServiceTests
{
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly AppUser _user;

    public TokenServiceTests()
    {
        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _user = new AppUser
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = new byte[] { 1 },
            PasswordSalt = new byte[] { 2 }
        };
    }

    private TokenService CreateTokenServiceWithKey(string tokenKey, int expiryDays = 7)
    {
        _jwtSettingsMock.Setup(x => x.Value).Returns(new JwtSettings
        {
            TokenKey = tokenKey,
            TokenExpiryDays = expiryDays
        });

        return new TokenService(_jwtSettingsMock.Object);
    }

    [Fact]
    public void CreateTokenAsync_WhenTokenKeyIsNull_ShouldReturnFailure()
    {
        // Arrange
        var tokenService = CreateTokenServiceWithKey(null!);

        // Act
        var result = tokenService.CreateToken(_user);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessages.TokenAccessError, result.Message);
        Assert.Null(result.Token);
    }

    [Fact]
    public void CreateTokenAsync_WhenTokenKeyIsTooShort_ShouldReturnFailure()
    {
        // Arrange
        var tokenService = CreateTokenServiceWithKey("shortkey");

        // Act
        var result = tokenService.CreateToken(_user);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ErrorMessages.InvalidToken, result.Message);
        Assert.Null(result.Token);
    }

    [Fact]
    public void CreateTokenAsync_WhenTokenKeyIsValid_ShouldReturnSuccess()
    {
        // Arrange
        var tokenService = CreateTokenServiceWithKey("supersecuretokenkeywithatleast64characterslongstringforjwtgeneration123");

        // Act
        var result = tokenService.CreateToken(_user);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(SuccessMessages.TokenCreated, result.Message);
        Assert.False(string.IsNullOrEmpty(result.Token));
    }
}
