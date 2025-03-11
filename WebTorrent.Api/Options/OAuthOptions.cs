namespace WebTorrent.Api.Options;
public abstract class OAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
}