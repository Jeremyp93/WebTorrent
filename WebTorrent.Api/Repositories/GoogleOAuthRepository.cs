using System.Web;
using WebTorrent.Api.Models;
using WebTorrent.Api.Options;
using WebTorrent.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace WebTorrent.Api.Repositories;
public interface IGoogleOAuthRepository
{
    string GetGoogleOAuthUrl();
    Task<Result<ApplicationUser>> GetGoogleUser(string code);
}

public class GoogleOAuthRepository : IGoogleOAuthRepository
{
    private readonly GoogleOptions _googleConfiguration;
    private readonly IGoogleClient _googleClient;
    private readonly UserManager<ApplicationUser> _userManager;
    private const string GoogleKey = "google";

    public GoogleOAuthRepository(IOptions<GoogleOptions> options, IGoogleClient googleClient, UserManager<ApplicationUser> userManager)
    {
        _googleConfiguration = options.Value;
        _googleClient = googleClient;
        _userManager = userManager;
    }

    public string GetGoogleOAuthUrl()
    {
        return $"{_googleConfiguration.AuthorizationEndpoint}?response_type=code&client_id={_googleConfiguration.ClientId}&redirect_uri={_googleConfiguration.CallbackUrl}&scope={HttpUtility.UrlEncode($"{_googleConfiguration.Scope}")}";
    }

    public async Task<Result<ApplicationUser>> GetGoogleUser(string code)
    {
        var googleUser = await _googleClient.GetUser(code);
        if (googleUser is null || string.IsNullOrEmpty(googleUser.Email))
        {
            return Result<ApplicationUser>.Failure(ResultStatusCode.ValidationError, "User info could not be retrieved.");
        }

        var existingUser = await _userManager.FindByEmailAsync(googleUser.Email.ToLower());
        if (existingUser is not null && existingUser.OAuthProviders.Any(o => o.OAuthProviderId == googleUser.Id && o.OAuthProviderName == GoogleKey))
        {
            return Result<ApplicationUser>.Success(existingUser);
        }

        var provider = OAuthProvider.Create(googleUser.Id, GoogleKey);
        if (existingUser is not null)
        {
            existingUser.OAuthProviders.Add(provider);
            var updateResult = await _userManager.UpdateAsync(existingUser);
            return updateResult.Succeeded ? Result<ApplicationUser>.Success(existingUser) : Result<ApplicationUser>.Failure(ResultStatusCode.Error, "User could not been logged in.");
        }

        var newUser = new ApplicationUser
        {
            UserName = googleUser.Email,
            Email = googleUser.Email,
            FirstName = googleUser.Name,
        };
        newUser.OAuthProviders.Add(provider);

        var createResult = await _userManager.CreateAsync(newUser);
        if (!createResult.Succeeded)
        {
            return Result<ApplicationUser>.Failure(ResultStatusCode.Error, "User could not been created.");
        }

        return Result<ApplicationUser>.Success(newUser);
    }
}