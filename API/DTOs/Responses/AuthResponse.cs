namespace API.DTOs.Responses;

public class AuthResponse : BaseResponse
{
    public string? UserEmail { get; set; }
    public string? Token { get; set; }
}
