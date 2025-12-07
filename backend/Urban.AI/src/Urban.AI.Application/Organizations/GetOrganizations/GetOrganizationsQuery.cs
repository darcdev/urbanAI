namespace Urban.AI.Application.Organizations.GetOrganizations;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Organizations.Dtos;
#endregion

public sealed record GetOrganizationsQuery(
    int PageNumber,
    int PageSize) : IQuery<PagedOrganizationsResponse>;
