using System.Security.Claims;

namespace WebTorrent.Api.Repositories;
public interface IClaimReader
{
    Guid GetUserIdFromClaim();

    string GetUsernameFromClaim();
}

public class ClaimReader : IClaimReader
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimReader(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserIdFromClaim()
    {
        if (_httpContextAccessor.HttpContext is null)
            return Guid.Empty;

        if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
        {
            // Accessing individual claims
            var userId = claimsIdentity.FindFirst("user_id")?.Value;
            return userId is null ? Guid.Empty : Guid.Parse(userId);
        }

        return Guid.Empty;
    }

    public string GetUsernameFromClaim()
    {
        if (_httpContextAccessor.HttpContext is null)
            return string.Empty;

        if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
        {
            // Accessing individual claims
            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            return email ?? string.Empty;
        }

        return string.Empty;
    }
}