namespace Urban.AI.Infrastructure.Auth.Authentication;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
#endregion

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Email =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetEmail() ??
        throw new ApplicationException("User context is unavailable");

    public string IdentityId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetIdentityId() ??
        throw new ApplicationException("User context is unavailable");

    public List<string> Roles =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserRoles() ?? [];
}
