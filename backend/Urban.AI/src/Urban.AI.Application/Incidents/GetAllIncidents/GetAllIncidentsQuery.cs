namespace Urban.AI.Application.Incidents.GetAllIncidents;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Incidents.Dtos;
#endregion

public record GetAllIncidentsQuery : IQuery<List<IncidentResponse>>;
