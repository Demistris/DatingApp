using API.Entities;

namespace API.DTOs.Responses;

public class LoginResponse : BaseResponse
{
    public AppUser? User { get; set; }
}
