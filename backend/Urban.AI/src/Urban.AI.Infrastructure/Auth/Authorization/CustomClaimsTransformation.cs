namespace Urban.AI.Infrastructure.Auth.Authorization;

#region Usings
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
#endregion

internal sealed class CustomClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        return Task.FromResult(principal);
    }
}
