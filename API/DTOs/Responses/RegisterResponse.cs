using API.Entities;

namespace API.DTOs.Responses;

public class RegisterResponse : BaseResponse
{
    public AppUser? User { get; set; }
}
