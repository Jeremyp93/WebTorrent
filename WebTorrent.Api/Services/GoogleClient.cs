using System.Text.Json;
using System.Text.Json.Serialization;
using WebTorrent.Api.Models;
using WebTorrent.Api.Options;
using Microsoft.Extensions.Options;

namespace WebTorrent.Api.Services;
public interface IGoogleClient
{
    Task<GoogleUser?> GetUser(string code);
}

public class GoogleClient : IGoogleClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleOptions _options;

    public GoogleClient(HttpClient googleHttpClient, IHttpClientFactory httpClientFactory, IOptions<GoogleOptions> options)
    {
        _httpClient = googleHttpClient;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
     }

    public async Task<GoogleUser?> GetUser(string code)
    {
        var token = await Authenticate(code);

        if (token is null)
        {
            return default;
        }

        // Set headers
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        _httpClient.DefaultRequestHeaders.Add("Client-Id", _options.ClientId);

        var response = await _httpClient.GetAsync("userinfo?alt=json");
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GoogleUser>(responseContent);
    }

    private async Task<string?> Authenticate(string code)
    {
        using var client = _httpClientFactory.CreateClient();

        var formData = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("client_id", _options.ClientId),
            new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("redirect_uri", _options.CallbackUrl)
        };

        var content = new FormUrlEncodedContent(formData);
        var response = await client.PostAsync(_options.TokenEndpoint, content);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(responseContent);
            if (tokenResponse?.IsSuccess ?? false)
            {
                return tokenResponse.AccessToken;
            }

            throw new Exception(tokenResponse?.error_description ?? "Unknown error during authentication");
        }

        return null;
    }
}

public class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;
    public string error { get; set; } = string.Empty;
    public string error_description { get; set; } = string.Empty;
    public bool IsSuccess => string.IsNullOrEmpty(error);
}