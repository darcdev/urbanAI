namespace Urban.AI.Domain.Organizations;

using Urban.AI.Domain.Common.Abstractions;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<Organization?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Organization>> GetAllActiveOrganizationsAsync(CancellationToken cancellationToken);
    Task<(IEnumerable<Organization> Organizations, int TotalCount)> GetOrganizationsWithPaginationAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
