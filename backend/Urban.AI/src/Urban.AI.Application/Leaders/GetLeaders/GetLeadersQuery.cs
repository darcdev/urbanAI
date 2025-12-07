namespace Urban.AI.Application.Leaders.GetLeaders;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Leaders.Dtos;
#endregion

public sealed record GetLeadersQuery(
    int PageNumber,
    int PageSize,
    Guid? DepartmentId,
    Guid? MunicipalityId) : IQuery<PagedLeadersResponse>;
