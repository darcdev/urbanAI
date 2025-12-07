namespace Urban.AI.Domain.Incidents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents.Resources;
#endregion

public static class IncidentErrors
{
    public static readonly Error NotFound = new(
        nameof(IncidentResources.IncidentNotFound),
        IncidentResources.IncidentNotFound);

    public static readonly Error ImageRequired = new(
        nameof(IncidentResources.IncidentImageRequired),
        IncidentResources.IncidentImageRequired);

    public static readonly Error LocationRequired = new(
        nameof(IncidentResources.IncidentLocationRequired),
        IncidentResources.IncidentLocationRequired);

    public static readonly Error InvalidImageFormat = new(
        nameof(IncidentResources.IncidentInvalidImageFormat),
        IncidentResources.IncidentInvalidImageFormat);

    public static Error ImageTooLarge(int maxSizeMb) => new(
        nameof(IncidentResources.IncidentImageTooLarge),
        string.Format(IncidentResources.IncidentImageTooLarge, maxSizeMb));

    public static readonly Error AnalysisFailed = new(
        nameof(IncidentResources.IncidentAnalysisFailed),
        IncidentResources.IncidentAnalysisFailed);

    public static readonly Error NoLeaderAvailable = new(
        nameof(IncidentResources.IncidentNoLeaderAvailable),
        IncidentResources.IncidentNoLeaderAvailable);

    public static readonly Error InvalidEmail = new(
        nameof(IncidentResources.IncidentInvalidEmail),
        IncidentResources.IncidentInvalidEmail);

    public static readonly Error MunicipalityNotFound = new(
        nameof(IncidentResources.IncidentMunicipalityNotFound),
        IncidentResources.IncidentMunicipalityNotFound);
}
