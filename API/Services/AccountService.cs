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
    private readonly ITokenService _tokenService;

    public AccountService(IAccountRepository accountRepository, ITokenService tokenService)
    {
        _accountRepository = accountRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
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

        var token = _tokenService.CreateToken(user);
        if (token == null || string.IsNullOrEmpty(token.Token))
        {
            return new AuthResponse
            {
                Success = false,
                Message = token?.Message ?? ErrorMessages.TokenAccessError
            };
        }

        return new AuthResponse
        {
            Success = true,
            Message = SuccessMessages.UserRegistered,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserEmail = user.Email,
            Token = token.Token
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = NormalizeEmail(request.Email);
        var user = await _accountRepository.GetUserByEmailAsync(normalizedEmail);
        if (user == null)
        {
            return new AuthResponse
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
                return new AuthResponse
                {
                    Success = false,
                    Message = ErrorMessages.InvalidPassword
                };
            }
        }

        var token = _tokenService.CreateToken(user);
        if (token == null || string.IsNullOrEmpty(token.Token))
        {
            return new AuthResponse
            {
                Success = false,
                Message = token?.Message ?? ErrorMessages.TokenAccessError
            };
        }

        return new AuthResponse
        {
            Success = true,
            Message = SuccessMessages.UserLoggedIn,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserEmail = user.Email,
            Token = token.Token
        };
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        var normalizedEmail = NormalizeEmail(email);
        return await _accountRepository.UserExistsAsync(normalizedEmail);
    }

    private string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
