namespace Urban.AI.Infrastructure.Auth;

using Urban.AI.Infrastructure.Auth.Authentication.Models;

internal static class AuthExtensions
{
    internal static Application.UserManagement.Users.Dtos.UserResponse ToUserResponse(this UserRepresentationModel keycloakUser)
    {
        return new Application.UserManagement.Users.Dtos.UserResponse(
            Guid.TryParse(keycloakUser.Id, out var parsedId) ? parsedId : Guid.Empty,
            keycloakUser.Email,
            keycloakUser.FirstName,
            keycloakUser.LastName,
            keycloakUser.EmailVerified ?? false,
            keycloakUser.Enabled ?? false,
            keycloakUser.CreatedTimestamp.HasValue
                ? DateTimeOffset.FromUnixTimeMilliseconds(keycloakUser.CreatedTimestamp.Value).UtcDateTime
                : DateTime.MinValue,
            keycloakUser.RealmRoles ?? Array.Empty<string>()
        );
    }

    internal static Application.UserManagement.Users.Dtos.RoleResponse ToRoleResponse(this RoleRepresentationModel keycloakRole)
    {
        return new Application.UserManagement.Users.Dtos.RoleResponse(
                keycloakRole.Id,
                keycloakRole.Name,
                keycloakRole.Description ?? string.Empty,
                keycloakRole.Composite,
                keycloakRole.ClientRole
        );
    }
}
