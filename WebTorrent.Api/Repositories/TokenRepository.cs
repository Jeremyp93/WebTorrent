using System.Security.Cryptography;
using LouisManager.Api.Extensions;
using LouisManager.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace LouisManager.Api.Repositories;
public interface ITokenRepository
{
    Task<Result<AuthToken>> Generate();
    Task<Result<ApplicationUser>> Validate(string token);
}

public class TokenRepository : ITokenRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IClaimReader _claimReader;

    public TokenRepository(UserManager<ApplicationUser> userManager, IClaimReader claimReader)
    {
        _userManager = userManager;
        _claimReader = claimReader;
    }


    public async Task<Result<AuthToken>> Generate()
    {
        var userId = _claimReader.GetUserIdFromClaim();
        var existingUser = await _userManager.FindByIdAsync(userId.ToString());
        if (existingUser is null)
        {
            return Result<AuthToken>.Failure(ResultStatusCode.ValidationError, "User info could not be retrieved.");
        }
        string token = GetRandomlyGenerateBase64String(15);
        var authToken = new AuthToken
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(120)
        };
        existingUser.AuthTokens.Add(authToken);
        var updateResult = await _userManager.UpdateAsync(existingUser);
        return updateResult.Succeeded ? Result<AuthToken>.Success(authToken) : Result<AuthToken>.Failure(ResultStatusCode.Error, "Token could not be created.");
    }

    public async Task<Result<ApplicationUser>> Validate(string token)
    {
        var existingUser = _userManager.FindByToken(token);
        if (existingUser is null)
        {
            return Result<ApplicationUser>.Failure(ResultStatusCode.ValidationError, "Token not valid.");
        }
        existingUser.AuthTokens.RemoveAll(t => t.Token == token);
        var updateResult = await _userManager.UpdateAsync(existingUser);
        return Result<ApplicationUser>.Success(existingUser);
    }

    private static string GetRandomlyGenerateBase64String(int count)
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(count);
        return Base64UrlTextEncoder.Encode(randomBytes);
    }
}