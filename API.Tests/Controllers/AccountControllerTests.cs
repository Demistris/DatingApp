using API.Controllers;
using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Messages;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Tests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly AccountController _accountController;
    private readonly RegisterRequest _validRegisterRequest;
    private readonly LoginRequest _validLoginRequest;

    public AccountControllerTests()
    {
        _accountServiceMock = new Mock<IAccountService>();
        _accountController = new AccountController(_accountServiceMock.Object);

        _validRegisterRequest = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "johndoe@example.com",
            Password = "Password123!"
        };

        _validLoginRequest = new LoginRequest
        {
            Email = "johndoe@example.com",
            Password = "Password123!"
        };
    }

    [Fact]
    public async Task Register_WhenUserAlreadyExists_ShouldReturnBadRequest()
    {
        // Arrange
        _accountServiceMock.Setup(s => s.UserExistsAsync(_validRegisterRequest.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _accountController.Register(_validRegisterRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.EmailAlreadyTaken, badRequestResult.Value);
    }

    [Fact]
    public async Task Register_WhenResponseIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        _accountServiceMock.Setup(s => s.UserExistsAsync(_validRegisterRequest.Email))
            .ReturnsAsync(false);
        _accountServiceMock.Setup(s => s.RegisterAsync(_validRegisterRequest))
            .ReturnsAsync((AuthResponse)null);

        // Act
        var result = await _accountController.Register(_validRegisterRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.InvalidRegister, badRequestResult.Value);
    }

    [Fact]
    public async Task Register_WhenServiceFails_ShouldReturnBadRequest()
    {
        // Arrange
        var failedResponse = new AuthResponse
        {
            Success = false,
            Message = "Service failed message"
        };

        _accountServiceMock.Setup(s => s.UserExistsAsync(_validRegisterRequest.Email))
            .ReturnsAsync(false);
        _accountServiceMock.Setup(s => s.RegisterAsync(_validRegisterRequest))
            .ReturnsAsync(failedResponse);

        // Act
        var result = await _accountController.Register(_validRegisterRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(failedResponse.Message, badRequestResult.Value);
    }

    [Theory]
    [InlineData(null, "token")]
    [InlineData("johndoe@example.com", null)]
    [InlineData(null, null)]
    public async Task Register_WhenEmailOrTokenIsNull_ShouldReturnBadRequest(string userEmail, string token)
    {
        // Arrange
        var invalidResponse = new AuthResponse
        {
            Success = true,
            Message = ErrorMessages.InvalidRegister,
            UserEmail = userEmail,
            Token = token
        };

        _accountServiceMock.Setup(s => s.UserExistsAsync(_validRegisterRequest.Email))
            .ReturnsAsync(false);
        _accountServiceMock.Setup(s => s.RegisterAsync(_validRegisterRequest))
            .ReturnsAsync(invalidResponse);

        // Act
        var result = await _accountController.Register(_validRegisterRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.InvalidRegister, badRequestResult.Value);
    }

    [Fact]
    public async Task Register_WhenRegistrationSucceeds_ShouldReturnOk()
    {
        // Arrange
        var successResponse = new AuthResponse
        {
            Success = true,
            Message = SuccessMessages.UserRegistered,
            UserEmail = _validRegisterRequest.Email,
            Token = "valid_token"
        };

        _accountServiceMock.Setup(s => s.UserExistsAsync(_validRegisterRequest.Email))
            .ReturnsAsync(false);
        _accountServiceMock.Setup(s => s.RegisterAsync(_validRegisterRequest))
            .ReturnsAsync(successResponse);

        // Act
        var result = await _accountController.Register(_validRegisterRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResponse = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.True(returnedResponse.Success);
        Assert.Equal(successResponse.UserEmail, returnedResponse.UserEmail);
        Assert.Equal(successResponse.Token, returnedResponse.Token);
    }

    [Fact]
    public async Task Login_WhenResponseIsNull_ShouldReturnUnauthorized()
    {
        // Arrange
        _accountServiceMock.Setup(s => s.LoginAsync(_validLoginRequest))
            .ReturnsAsync((AuthResponse)null);

        // Act
        var result = await _accountController.Login(_validLoginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.InvalidLogin, unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_WhenServiceFails_ShouldReturnUnauthorizedWithMessage()
    {
        // Arrange
        var failedResponse = new AuthResponse
        {
            Success = false,
            Message = "Service failed message"
        };

        _accountServiceMock.Setup(s => s.LoginAsync(_validLoginRequest))
            .ReturnsAsync(failedResponse);

        // Act
        var result = await _accountController.Login(_validLoginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal(failedResponse.Message, unauthorizedResult.Value);
    }

    [Theory]
    [InlineData(null, "token")]
    [InlineData("johndoe@example.com", null)]
    [InlineData(null, null)]
    public async Task Login_WhenEmailOrTokenIsNull_ShouldReturnUnauthorized(string userEmail, string token)
    {
        // Arrange
        var invalidResponse = new AuthResponse
        {
            Success = true,
            Message = ErrorMessages.InvalidLogin,
            UserEmail = userEmail,
            Token = token
        };

        _accountServiceMock.Setup(s => s.LoginAsync(_validLoginRequest))
            .ReturnsAsync(invalidResponse);

        // Act
        var result = await _accountController.Login(_validLoginRequest);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.InvalidLogin, unauthorizedResult.Value);
    }


    [Fact]
    public async Task Login_WhenLoginSucceeds_ShouldReturnOk()
    {
        // Arrange
        var successResponse = new AuthResponse
        {
            Success = true,
            Message = SuccessMessages.UserLoggedIn,
            UserEmail = _validLoginRequest.Email,
            Token = "valid_token"
        };

        _accountServiceMock.Setup(s => s.LoginAsync(_validLoginRequest))
            .ReturnsAsync(successResponse);

        // Act
        var result = await _accountController.Login(_validLoginRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResponse = Assert.IsType<AuthResponse>(okResult.Value);

        Assert.True(returnedResponse.Success);
        Assert.Equal(successResponse.UserEmail, returnedResponse.UserEmail);
        Assert.Equal(successResponse.Token, returnedResponse.Token);
    }
}
