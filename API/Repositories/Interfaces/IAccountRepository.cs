using API.Entities;

namespace API.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<AppUser?> GetUserByEmailAsync(string normalizedEmail);
    Task<bool> UserExistsAsync(string normalizedEmail);
    Task<AppUser> AddUserAsync(AppUser user);
}
