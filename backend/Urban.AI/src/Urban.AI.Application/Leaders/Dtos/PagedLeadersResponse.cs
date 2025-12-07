namespace Urban.AI.Application.Leaders.Dtos;

public sealed record PagedLeadersResponse(
    IEnumerable<LeaderResponse> Leaders,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);
