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

    public async Task<Incident?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
                .ThenInclude(l => l.User)
                    .ThenInclude(u => u.UserDetails)
            .Include(i => i.Category)
            .Include(i => i.Subcategory)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Incident?> GetByRadicateNumberAsync(string radicateNumber, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Category)
            .Include(i => i.Subcategory)
            .FirstOrDefaultAsync(i => i.RadicateNumber == radicateNumber, cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetByLeaderIdAsync(Guid leaderId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
                .ThenInclude(l => l.User)
                    .ThenInclude(u => u.UserDetails)
            .Include(i => i.Category)
            .Include(i => i.Subcategory)
            .Where(i => i.LeaderId == leaderId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetByMunicipalityIdAsync(Guid municipalityId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
            .Include(i => i.Category)
            .Include(i => i.Subcategory)
            .Where(i => i.MunicipalityId == municipalityId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
            .Include(i => i.Category)
            .Include(i => i.Subcategory)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetAllIncidentsAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
                .ThenInclude(l => l.User)
                    .ThenInclude(u => u.UserDetails)
            .Include(i => i.Category)
            .Include(i => i.Subcategory)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Incident>> GetByGeographyAsync(
        Guid? departmentId,
        Guid? municipalityId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<Incident>()
            .Include(i => i.Municipality)
            .Include(i => i.Leader)
            .Include(i => i.Category)
            .Include(i => i.Subcategory)
            .AsQueryable();

        if (municipalityId.HasValue)
        {
            query = query.Where(i => i.MunicipalityId == municipalityId.Value);
        }
        else if (departmentId.HasValue)
        {
            var departmentDaneCode = await _dbContext.Set<Urban.AI.Domain.Geography.Department>()
                .Where(d => d.Id == departmentId.Value)
                .Select(d => d.DepartmentDaneCode)
                .FirstOrDefaultAsync(cancellationToken);

            if (!string.IsNullOrEmpty(departmentDaneCode))
            {
                query = query.Where(i => i.Municipality.DepartmentDaneCode == departmentDaneCode);
            }
        }

        var incidents = await query
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

        foreach (var incident in incidents)
        {
            if (incident.Leader != null)
            {
                await _dbContext.Entry(incident.Leader)
                    .Reference(l => l.User)
                    .LoadAsync(cancellationToken);

                if (incident.Leader.User != null)
                {
                    await _dbContext.Entry(incident.Leader.User)
                        .Reference(u => u.UserDetails)
                        .LoadAsync(cancellationToken);
                }
            }
        }

        return incidents;
    }
}
