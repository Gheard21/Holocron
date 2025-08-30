using System.Security.Claims;
using Holocron.App.Api.Interfaces;

namespace Holocron.App.Api.Providers;

public class IdentityProvider(IHttpContextAccessor httpContextAccessor) : IIdentityProvider
{
    public string? GetCurrentUserId()
        => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
