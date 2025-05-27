using API.Entities;

namespace API.Repositories.Interfaces;

public interface IUsersRepository
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
}
