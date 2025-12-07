namespace Urban.AI.Application.Organizations.GetOrganizations;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Organizations.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Organizations;
#endregion

internal sealed class GetOrganizationsQueryHandler : IQueryHandler<GetOrganizationsQuery, PagedOrganizationsResponse>
{
    private readonly IOrganizationRepository _organizationRepository;

    public GetOrganizationsQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<Result<PagedOrganizationsResponse>> Handle(GetOrganizationsQuery request, CancellationToken cancellationToken)
    {
        var (organizations, totalCount) = await _organizationRepository.GetOrganizationsWithPaginationAsync(
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var organizationResponses = organizations.Select(org => org.ToOrganizationResponse());

        var pagedResponse = new PagedOrganizationsResponse(
            organizationResponses,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages);

        return Result.Success(pagedResponse);
    }
}
