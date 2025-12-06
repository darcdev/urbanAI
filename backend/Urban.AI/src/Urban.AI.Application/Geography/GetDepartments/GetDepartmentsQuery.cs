namespace Urban.AI.Application.Geography.GetDepartments;

using Urban.AI.Application.Geography.Dtos;
using Urban.AI.Application.Common.Abstractions.CQRS;

public record GetDepartmentsQuery : IQuery<IEnumerable<DepartmentResponse>>;