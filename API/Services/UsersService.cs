using API.Entities;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var users = await _usersRepository.GetUsersAsync();
        return users;
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        var user = await _usersRepository.GetUserByIdAsync(id);
        return user;
    }
}
