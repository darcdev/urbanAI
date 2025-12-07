namespace Urban.AI.Domain.Leaders;

using Urban.AI.Domain.Common.Abstractions;

public interface ILeaderRepository : IRepository<Leader>
{
    Task<Leader?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Leader?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Leader>> GetAllActiveLeadersAsync(CancellationToken cancellationToken);
    Task<Leader?> GetNearestLeaderAsync(decimal latitude, decimal longitude, CancellationToken cancellationToken);
    Task<(IEnumerable<Leader> Leaders, int TotalCount)> GetLeadersWithPaginationAsync(
        int pageNumber,
        int pageSize,
        Guid? departmentId,
        Guid? municipalityId,
        CancellationToken cancellationToken);
}
