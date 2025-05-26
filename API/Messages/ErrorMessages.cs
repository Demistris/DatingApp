namespace API.Messages;

public static class ErrorMessages
{
    public const string UserNotFound = "User not found";
    public const string EmailAlreadyTaken = "Email is already taken";
    public const string InvalidEmail = "Invalid email";
    public const string InvalidPassword = "Invalid password";
    public const string InvalidToken = "TokenKey must be at least 64 characters long";
    public const string TokenAccessError = "Cannot access tokenKey from configuration";
    public const string InvalidRegister = "Invalid register attempt";
    public const string InvalidLogin = "Invalid login attempt";
}
