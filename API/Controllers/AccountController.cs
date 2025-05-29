using API.DTOs.Requests;
using API.DTOs.Responses;
using API.Messages;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (await _accountService.UserExistsAsync(request.Email))
        {
            return BadRequest(ErrorMessages.EmailAlreadyTaken);
        }

        var response = await _accountService.RegisterAsync(request);

        if (!IsValidAuthResponse(response))
        {
            return BadRequest(response?.Message ?? ErrorMessages.InvalidRegister);
        }

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _accountService.LoginAsync(request);

        if (!IsValidAuthResponse(response))
        {
            return Unauthorized(response?.Message ?? ErrorMessages.InvalidLogin);
        }

        return Ok(response);
    }

    private bool IsValidAuthResponse(AuthResponse response)
    {
        if (response == null ||
            !response.Success ||
            string.IsNullOrWhiteSpace(response.FirstName) ||
            string.IsNullOrWhiteSpace(response.LastName) ||
            string.IsNullOrWhiteSpace(response.UserEmail) ||
            string.IsNullOrWhiteSpace(response.Token))
        {
            return false;
        }

        return true;
    }
}
