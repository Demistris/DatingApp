using API.Entities;

namespace API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
}
