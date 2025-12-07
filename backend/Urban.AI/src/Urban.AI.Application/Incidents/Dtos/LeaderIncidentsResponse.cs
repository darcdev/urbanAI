namespace Urban.AI.Application.Incidents.Dtos;

public sealed record LeaderIncidentsResponse(
    IEnumerable<IncidentResponse> Accepted,
    IEnumerable<IncidentResponse> Pending,
    IEnumerable<IncidentResponse> Rejected);
