namespace Urban.AI.Application.Incidents.GetIncidentsByGeography;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Incidents.Dtos;
#endregion

public record GetIncidentsByGeographyQuery(
    Guid? DepartmentId,
    Guid? MunicipalityId) : IQuery<List<IncidentResponse>>;
