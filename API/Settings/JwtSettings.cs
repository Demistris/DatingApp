namespace API.Settings;

public class JwtSettings
{
    public required string TokenKey { get; set; }
    public int TokenExpiryDays { get; set; }
}
