namespace Urban.AI.Infrastructure.Auth.Authorization;

using Urban.AI.Domain.Users;

#region Usings
#endregion

internal sealed class UserRolesResponse
{
    public Guid UserId { get; init; }

    public List<Role> Roles { get; init; } = [];
}
