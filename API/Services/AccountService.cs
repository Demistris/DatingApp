using System.Security.Cryptography;
using System.Text;
using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Entities;
using API.Messages;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
            PasswordSalt = hmac.Key
        };

        await _accountRepository.AddUserAsync(user);

        return new RegisterResponse
        {
            Success = true,
            Message = SuccessMessages.UserRegistered,
            User = user
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await _accountRepository.GetUserByEmailAsync(normalizedEmail);
        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                Message = ErrorMessages.InvalidEmail
            };
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = ErrorMessages.InvalidPassword
                };
            }
        }

        return new LoginResponse
        {
            Success = true,
            Message = SuccessMessages.UserLoggedIn,
            User = user
        };
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        return await _accountRepository.UserExistsAsync(normalizedEmail);
    }

    private string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
