using System.Security.Claims;

namespace ChatApp.Chat.Common.Extensions;


public static class ClaimsPrincipalExtension
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return claim is null
            ? null
            : Guid.Parse(claim);
    }
}
