namespace WebApiAuthor.DTOs;

public class AuthenticationResponse
{
    public string Token { get; set; }
    public DateTime ExpirationTime { get; set; }
}