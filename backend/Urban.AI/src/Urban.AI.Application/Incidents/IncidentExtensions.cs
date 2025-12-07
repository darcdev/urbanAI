namespace Urban.AI.Application.Incidents;

#region Usings
using Urban.AI.Application.Incidents.Dtos;
using Urban.AI.Domain.Incidents;
#endregion

internal static class IncidentExtensions
{
    public static IncidentResponse ToResponse(this Incident incident, string imageUrl)
    {
        return new IncidentResponse(
            incident.Id,
            incident.RadicateNumber,
            imageUrl,
            incident.Location.Latitude,
            incident.Location.Longitude,
            incident.CitizenEmail,
            incident.AdditionalComment,
            incident.Caption,
            incident.AiDescription,
            incident.Category?.ToString(),
            incident.Severity?.ToString(),
            incident.Status.ToString(),
            incident.Priority.ToString(),
            incident.CreatedAt);
    }
}
