namespace Urban.AI.Domain.Organizations;

using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Organizations.Resources;

public static class OrganizationErrors
{
    public static readonly Error OrganizationNotFound = new(
        nameof(OrganizationResources.OrganizationNotFound),
        OrganizationResources.OrganizationNotFound);

    public static readonly Error OrganizationAlreadyExists = new(
        nameof(OrganizationResources.OrganizationAlreadyExists),
        OrganizationResources.OrganizationAlreadyExists);

    public static readonly Error UserNotFound = new(
        nameof(OrganizationResources.UserNotFound),
        OrganizationResources.UserNotFound);

    public static readonly Error InvalidOrganizationName = new(
        nameof(OrganizationResources.InvalidOrganizationName),
        OrganizationResources.InvalidOrganizationName);

    public static readonly Error FailedToSendEmail = new(
        nameof(OrganizationResources.FailedToSendEmail),
        OrganizationResources.FailedToSendEmail);

    public static Error EmailAlreadyExists(string email) => new(
        nameof(OrganizationResources.EmailAlreadyExists),
        string.Format(OrganizationResources.EmailAlreadyExists, email));

    public static readonly Error FailedToCreateUserInKeycloak = new(
        nameof(OrganizationResources.FailedToCreateUserInKeycloak),
        OrganizationResources.FailedToCreateUserInKeycloak);
}
