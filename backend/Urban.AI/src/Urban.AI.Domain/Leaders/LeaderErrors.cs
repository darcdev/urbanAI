namespace Urban.AI.Domain.Leaders;

using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Leaders.Resources;

public static class LeaderErrors
{
    public static readonly Error LeaderNotFound = new(
        nameof(LeaderResources.LeaderNotFound),
        LeaderResources.LeaderNotFound);

    public static readonly Error LeaderAlreadyExists = new(
        nameof(LeaderResources.LeaderAlreadyExists),
        LeaderResources.LeaderAlreadyExists);

    public static readonly Error UserNotFound = new(
        nameof(LeaderResources.UserNotFound),
        LeaderResources.UserNotFound);

    public static readonly Error DepartmentNotFound = new(
        nameof(LeaderResources.DepartmentNotFound),
        LeaderResources.DepartmentNotFound);

    public static readonly Error MunicipalityNotFound = new(
        nameof(LeaderResources.MunicipalityNotFound),
        LeaderResources.MunicipalityNotFound);

    public static readonly Error InvalidCoordinates = new(
        nameof(LeaderResources.InvalidCoordinates),
        LeaderResources.InvalidCoordinates);

    public static readonly Error FailedToSendEmail = new(
        nameof(LeaderResources.FailedToSendEmail),
        LeaderResources.FailedToSendEmail);

    public static Error EmailAlreadyExists(string email) => new(
        nameof(LeaderResources.EmailAlreadyExists),
        string.Format(LeaderResources.EmailAlreadyExists, email));

    public static readonly Error FailedToCreateUserInKeycloak = new(
        nameof(LeaderResources.FailedToCreateUserInKeycloak),
        LeaderResources.FailedToCreateUserInKeycloak);
}
