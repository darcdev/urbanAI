namespace Urban.AI.Application.Incidents.GetLeaderIncidents;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Incidents.Dtos;
#endregion

public sealed record GetLeaderIncidentsQuery : IQuery<LeaderIncidentsResponse>;
