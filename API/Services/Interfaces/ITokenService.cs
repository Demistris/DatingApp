using API.DTOs.Responses;
using API.Entities;

namespace API.Services.Interfaces;

public interface ITokenService
{
    TokenResponse CreateToken(AppUser user);
}
