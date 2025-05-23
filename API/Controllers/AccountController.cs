using API.DTOs.Requests;
using API.Entities;
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
    public async Task<ActionResult<AppUser>> Register(RegisterRequest request)
    {
        if (await _accountService.UserExistsAsync(request.Email))
        {
            return BadRequest(ErrorMessages.EmailAlreadyTaken);
        }

        var response = await _accountService.RegisterAsync(request);

        if (!response.Success || response.User == null)
        {
            return BadRequest(response);
        }

        return Ok(response.User);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginRequest request)
    {    
        var response = await _accountService.LoginAsync(request);

        if (!response.Success || response.User == null) // access token is null
        {
            return Unauthorized(ErrorMessages.InvalidEmail); // or InvalidPassword
        }

        return Ok(response.User);
    }
}
