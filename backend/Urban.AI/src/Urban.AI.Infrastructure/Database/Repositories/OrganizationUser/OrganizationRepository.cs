namespace Urban.AI.Infrastructure.Database.Repositories.OrganizationUser;

#region Usings
using Microsoft.EntityFrameworkCore;
using Urban.AI.Domain.Common.Abstractions;
using DomainOrganization = Urban.AI.Domain.Organizations.Organization;
using IOrganizationRepository = Urban.AI.Domain.Organizations.IOrganizationRepository;
#endregion

internal sealed class OrganizationRepository : Repository<DomainOrganization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<DomainOrganization?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<DomainOrganization>()
            .FirstOrDefaultAsync(organization => organization.UserId == userId, cancellationToken);
    }

    public async Task<DomainOrganization?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<DomainOrganization>()
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<DomainOrganization>> GetAllActiveOrganizationsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<DomainOrganization>()
            .Include(e => e.User)
            .Where(e => e.IsActive)
            .ToListAsync(cancellationToken);
    }
}
