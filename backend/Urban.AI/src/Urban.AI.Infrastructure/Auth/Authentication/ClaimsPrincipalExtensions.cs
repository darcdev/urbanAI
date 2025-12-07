namespace Urban.AI.Infrastructure.Auth.Authentication;

#region Usings
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
#endregion

internal static class ClaimsPrincipalExtensions
{
    #region Constants
    private static readonly HashSet<string> AllowedRoles = ["Admin", "Organization", "Leader"];
    #endregion

    public static string GetEmail(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(JwtRegisteredClaimNames.Email) ??
               principal?.FindFirstValue(ClaimTypes.Email) ??
               throw new ApplicationException("User email is unavailable");
    }

    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        return principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
               principal?.FindFirstValue(ClaimTypes.NameIdentifier) ??
               throw new ApplicationException("User identity is unavailable");
    }

    public static List<string> GetUserRoles(this ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return [];
        }

        return principal.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .Where(role => AllowedRoles.Contains(role))
            .ToList();
    }
}
