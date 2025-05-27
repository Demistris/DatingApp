using API.Entities;

namespace API.Services.Interfaces;

public interface IUsersService
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
}
