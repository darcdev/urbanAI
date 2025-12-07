namespace Urban.AI.Application.Incidents.UpdateIncidentStatus;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Incidents;
#endregion

public record UpdateIncidentStatusCommand(
    Guid IncidentId,
    IncidentStatus Status,
    IncidentPriority? Priority) : ICommand;
