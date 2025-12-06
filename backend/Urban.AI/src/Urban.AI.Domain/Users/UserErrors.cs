namespace Urban.AI.Domain.Users;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users.Resources;
#endregion

public static class UserErrors
{
    public static readonly Error NotFound = new(
        nameof(UserResources.UserNotFound),
        UserResources.UserNotFound);

    public static readonly Error InvalidCredentials = new(
        nameof(UserResources.InvalidCredentials),
        UserResources.InvalidCredentials);

    public static Error EmailAlreadyExists(string email) => new(
        nameof(UserResources.EmailAlreadyExists),
        string.Format(UserResources.EmailAlreadyExists, email));

    public static Error RolesDoNotExist(string[] roles) => new(
        nameof(UserResources.RolesDoNotExist),
        string.Format(UserResources.RolesDoNotExist, string.Join(", ", roles)));

    public static readonly Error UserDetailsAlreadyCompleted = new(
        nameof(UserResources.UserDetailsAlreadyCompleted),
        UserResources.UserDetailsAlreadyCompleted);

    public static readonly Error UserDetailsNotCompleted = new(
        nameof(UserResources.UserDetailsNotCompleted),
        UserResources.UserDetailsNotCompleted);
}