using API.DTOs.Requests;
using API.DTOs.Responses;

namespace API.Services.Interfaces;

public interface IAccountService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<bool> UserExistsAsync(string email);
}
