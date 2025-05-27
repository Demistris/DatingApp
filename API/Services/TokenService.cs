using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs.Responses;
using API.Entities;
using API.Messages;
using API.Services.Interfaces;
using API.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public TokenResponse CreateToken(AppUser user)
    {
        var tokenKey = _jwtSettings.TokenKey;
        if (string.IsNullOrEmpty(tokenKey))
        {
            return new TokenResponse
            {
                Success = false,
                Message = ErrorMessages.TokenAccessError
            };
        }

        if (tokenKey.Length < 64)
        {
            return new TokenResponse
            {
                Success = false,
                Message = ErrorMessages.InvalidToken
            };
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Email)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.TokenExpiryDays),
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new TokenResponse
        {
            Success = true,
            Message = SuccessMessages.TokenCreated,
            Token = tokenHandler.WriteToken(token)
        };
    }
}
