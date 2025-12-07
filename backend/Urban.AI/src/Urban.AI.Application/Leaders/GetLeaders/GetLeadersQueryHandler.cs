namespace Urban.AI.Application.Leaders.GetLeaders;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Leaders.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Leaders;
#endregion

internal sealed class GetLeadersQueryHandler : IQueryHandler<GetLeadersQuery, PagedLeadersResponse>
{
    private readonly ILeaderRepository _leaderRepository;

    public GetLeadersQueryHandler(ILeaderRepository leaderRepository)
    {
        _leaderRepository = leaderRepository;
    }

    public async Task<Result<PagedLeadersResponse>> Handle(GetLeadersQuery request, CancellationToken cancellationToken)
    {
        var (leaders, totalCount) = await _leaderRepository.GetLeadersWithPaginationAsync(
            request.PageNumber,
            request.PageSize,
            request.DepartmentId,
            request.MunicipalityId,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var leaderResponses = leaders.Select(leader => leader.ToLeaderResponse());

        var pagedResponse = new PagedLeadersResponse(
            leaderResponses,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages);

        return Result.Success(pagedResponse);
    }
}
