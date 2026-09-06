namespace NewsHub.Application.Site.DTOs.Auth;

public class AuthResult
{
    public bool Success { get; }
    public string Message { get; }

    public AuthResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

}