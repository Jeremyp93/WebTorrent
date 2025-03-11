using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace LouisManager.Api.Models;

[CollectionName("User")]
public class ApplicationUser : MongoIdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<OAuthProvider> OAuthProviders { get; set; } = [];
    public List<AuthToken> AuthTokens { get; set; } = [];
}