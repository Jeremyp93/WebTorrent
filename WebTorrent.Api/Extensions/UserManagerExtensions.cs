using LouisManager.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace LouisManager.Api.Extensions;

public static class UserManagerExtensions
{
    public static ApplicationUser? FindByToken(this UserManager<ApplicationUser> userManager, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        // Query users for the one containing the matching token
        var user = userManager.Users
            .FirstOrDefault(user =>
                user.AuthTokens.Any(authToken =>
                    authToken.Token == token &&
                    authToken.Expiration > DateTime.UtcNow));

        return user;
    }
}