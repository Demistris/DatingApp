using API.Data;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class AccountRepository(DataContext context) : IAccountRepository
{
    public async Task<AppUser?> GetUserByEmailAsync(string normalizedEmail)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail);
    }

    public async Task<bool> UserExistsAsync(string normalizedEmail)
    {
        return await context.Users.AnyAsync(x => x.Email == normalizedEmail);
    }

    public async Task<AppUser> AddUserAsync(AppUser user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user;
    }
}
