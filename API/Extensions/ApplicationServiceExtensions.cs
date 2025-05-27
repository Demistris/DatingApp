using API.Data;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Services;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();

        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}
