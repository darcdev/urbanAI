namespace Urban.AI.WebApi.Controllers.Incidents.Dtos;

#region Usings
using Urban.AI.Domain.Incidents;
#endregion

public record AcceptIncidentRequest(IncidentPriority Priority);
