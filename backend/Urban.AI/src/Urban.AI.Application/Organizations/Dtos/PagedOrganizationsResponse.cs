namespace Urban.AI.Application.Organizations.Dtos;

public sealed record PagedOrganizationsResponse(
    IEnumerable<OrganizationResponse> Organizations,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);
