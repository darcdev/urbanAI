namespace Urban.AI.Application.Geography.GetMunicipalitiesByDepartment;

using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Geography.Dtos;

public record GetMunicipalitiesByDepartmentQuery(
    string DepartmentDaneCode) : IQuery<IEnumerable<MunicipalityResponse>>;