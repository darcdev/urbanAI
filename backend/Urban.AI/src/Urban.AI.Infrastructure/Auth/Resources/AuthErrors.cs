namespace Urban.AI.Infrastructure.Auth.Resources;

using Urban.AI.Domain.Common.Abstractions;

public static class AuthErrors
{
    public static readonly Error AuthenticationFailed = new(
        nameof(KeycloakResources.AuthenticationFailed),
        KeycloakResources.AuthenticationFailed);

    public static readonly Error FailedToRetrieveUsers = new(
        nameof(KeycloakResources.FailedToRetrieveUsers),
        KeycloakResources.FailedToRetrieveUsers);

    public static readonly Error FailedToRetrieveUser = new(
        nameof(KeycloakResources.FailedToRetrieveUser),
        KeycloakResources.FailedToRetrieveUser);

    public static readonly Error UserNotFoundInKeycloak = new(
        nameof(KeycloakResources.UserNotFoundInKeycloak),
        KeycloakResources.UserNotFoundInKeycloak);

    public static readonly Error FailedToUpdateUser = new(
        nameof(KeycloakResources.FailedToUpdateUser),
        KeycloakResources.FailedToUpdateUser);

    public static readonly Error FailedToDeleteUser = new(
        nameof(KeycloakResources.FailedToDeleteUser),
        KeycloakResources.FailedToDeleteUser);

    public static readonly Error FailedToAssignRoles = new(
        nameof(KeycloakResources.FailedToAssignRoles),
        KeycloakResources.FailedToAssignRoles);

    public static readonly Error FailedToRetrieveUserRoles = new(
        nameof(KeycloakResources.FailedToRetrieveUserRoles),
        KeycloakResources.FailedToRetrieveUserRoles);

    public static readonly Error FailedToRetrieveRoles = new(
        nameof(KeycloakResources.FailedToRetrieveRoles),
        KeycloakResources.FailedToRetrieveRoles);

    public static readonly Error FailedToRetrieveRole = new(
        nameof(KeycloakResources.FailedToRetrieveRole),
        KeycloakResources.FailedToRetrieveRole);

    public static readonly Error FailedToCreateRole = new(
        nameof(KeycloakResources.FailedToCreateRole),
        KeycloakResources.FailedToCreateRole);

    public static readonly Error FailedToUpdateRole = new(
        nameof(KeycloakResources.FailedToUpdateRole),
        KeycloakResources.FailedToUpdateRole);

    public static readonly Error FailedToDeleteRole = new(
        nameof(KeycloakResources.FailedToDeleteRole),
        KeycloakResources.FailedToDeleteRole);

    public static Error RoleNotFound(string roleName) => new(
        nameof(KeycloakResources.RoleNotFound),
        string.Format(KeycloakResources.RoleNotFound, roleName));

    public static Error RoleAlreadyExists(string roleName) => new(
        nameof(KeycloakResources.RoleAlreadyExists),
        string.Format(KeycloakResources.RoleAlreadyExists, roleName));
}
