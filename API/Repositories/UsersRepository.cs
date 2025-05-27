using API.Data;
using API.Entities;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class UsersRepository(DataContext context) : IUsersRepository
{
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }
}
