namespace Urban.AI.Application.Incidents.RejectIncident;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
#endregion

public record RejectIncidentCommand(Guid IncidentId) : ICommand;
