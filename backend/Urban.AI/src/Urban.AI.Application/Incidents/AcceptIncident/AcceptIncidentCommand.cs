namespace Urban.AI.Application.Incidents.AcceptIncident;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Incidents;
#endregion

public record AcceptIncidentCommand(Guid IncidentId, IncidentPriority Priority) : ICommand;
