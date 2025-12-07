namespace Urban.AI.Infrastructure.Database.Repositories.Incident;

#region Usings
using Urban.AI.Domain.Incidents;
using Microsoft.EntityFrameworkCore;
#endregion

internal sealed class IncidentRepository : Repository<Incident>, IIncidentRepository
{
    public IncidentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Incident?> GetByRadicateNumberAsync(string radicateNumber, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .FirstOrDefaultAsync(i => i.RadicateNumber == radicateNumber, cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetByLeaderIdAsync(Guid leaderId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
            .Where(i => i.LeaderId == leaderId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetByMunicipalityIdAsync(Guid municipalityId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
            .Where(i => i.MunicipalityId == municipalityId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
