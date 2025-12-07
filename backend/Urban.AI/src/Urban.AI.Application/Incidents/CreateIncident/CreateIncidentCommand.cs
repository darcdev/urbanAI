namespace Urban.AI.Application.Incidents.CreateIncident;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Incidents.Dtos;
#endregion

public sealed record CreateIncidentCommand(CreateIncidentRequest Request) : ICommand<IncidentResponse>;
