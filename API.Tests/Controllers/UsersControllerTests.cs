using API.Controllers;
using API.Entities;
using API.Messages;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUsersService> _usersServiceMock;
    private readonly UsersController _usersController;

    public UsersControllerTests()
    {
        _usersServiceMock = new Mock<IUsersService>();
        _usersController = new UsersController(_usersServiceMock.Object);
    }

    [Fact]
    public async Task GetUsers_WhenServiceSucceeds_ShouldReturnOk()
    {
        // Arrange
        var users = new List<AppUser>
        {
            new AppUser
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PasswordHash = new byte[] { 1, 2, 3 },
                PasswordSalt = new byte[] { 4, 5, 6 }
            },
            new AppUser
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                PasswordHash = new byte[] { 7, 8, 9 },
                PasswordSalt = new byte[] { 10, 11, 12 }
            }
        };

        _usersServiceMock.Setup(service => service.GetUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _usersController.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<AppUser>>(okResult.Value);
        Assert.Equal(2, ((List<AppUser>)returnedUsers).Count);
    }

    [Fact]
    public async Task GetUsers_WhenServiceReturnsEmptyList_ShouldReturnOk()
    {
        // Arrange
        var users = new List<AppUser>();

        _usersServiceMock.Setup(service => service.GetUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _usersController.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<AppUser>>(okResult.Value);
        Assert.Empty(returnedUsers);
    }

    [Fact]
    public async Task GetUserById_WhenUserExists_ShouldReturnOk()
    {
        // Arrange 
        var user = new AppUser
        {
            Id = 1,
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            PasswordHash = new byte[] { 1, 2 },
            PasswordSalt = new byte[] { 3, 4 }
        };

        _usersServiceMock.Setup(s => s.GetUserByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _usersController.GetUserById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<AppUser>(okResult.Value);
        Assert.Equal("alice@example.com", returnedUser.Email);
    }

    [Fact]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        _usersServiceMock.Setup(s => s.GetUserByIdAsync(2))
            .ReturnsAsync((AppUser)null);

        // Act
        var result = await _usersController.GetUserById(2);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal(ErrorMessages.UserNotFound, notFoundResult.Value);
    }
}
