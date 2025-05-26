namespace API.DTOs.Responses;

public class BaseResponse
{
    public bool Success { get; set; }
    public required string Message { get; set; }
}
