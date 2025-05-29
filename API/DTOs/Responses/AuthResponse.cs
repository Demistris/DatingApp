namespace API.DTOs.Responses;

public class AuthResponse : BaseResponse
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserEmail { get; set; }
    public string? Token { get; set; }
}
