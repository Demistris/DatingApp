using API.Entities;
using API.Repositories.Interfaces;
using API.Services;
using Moq;

namespace API.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly UsersService _usersService;

    public UserServiceTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _usersService = new UsersService(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUsersAsync_WhenUsersExist_ShouldReturnListOfUsers()
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
                PasswordHash = new byte[] {1},
                PasswordSalt = new byte[] {2}
            },
            new AppUser
            {
                Id = 2,
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                PasswordHash = new byte[] {3},
                PasswordSalt = new byte[] {4}
            }
        };

        _usersRepositoryMock.Setup(repo => repo.GetUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _usersService.GetUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<AppUser>)result).Count);
    }

    [Fact]
    public async Task GetUsersAsync_WhenNoUsersExist_ShouldReturnEmptyList()
    {
        // Arrange
        _usersRepositoryMock.Setup(repo => repo.GetUsersAsync())
            .ReturnsAsync(new List<AppUser>());

        // Act
        var result = await _usersService.GetUsersAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new AppUser
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = new byte[] { 1 },
            PasswordSalt = new byte[] { 2 }
        };

        _usersRepositoryMock.Setup(repo => repo.GetUserByIdAsync(1))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _usersService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
        Assert.Equal("john@example.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _usersRepositoryMock.Setup(repo => repo.GetUserByIdAsync(999))
            .ReturnsAsync((AppUser?)null);

        // Act
        var result = await _usersService.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}
