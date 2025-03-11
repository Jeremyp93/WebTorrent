namespace LouisManager.Api.Models;

public class AuthToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}