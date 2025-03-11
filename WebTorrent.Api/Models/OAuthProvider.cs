using LouisManager.Api.Exceptions;

namespace LouisManager.Api.Models;
public record OAuthProvider
{
    public string OAuthProviderId { get; private set; } = string.Empty;
    public string OAuthProviderName { get; private set; } = string.Empty;

    private OAuthProvider() { }

    public static OAuthProvider Create(string providerId, string providerName)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(providerId))
        {
            errors.Add($"ProviderId cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(providerName))
        {
            errors.Add($"ProviderName cannot be null or empty.");
        }

        if (errors.Count != 0)
        {
            throw new BusinessValidationException("OAuthProvider is invalid", errors);
        }

        return new OAuthProvider()
        {
            OAuthProviderId = providerId,
            OAuthProviderName = providerName
        };
    }
}