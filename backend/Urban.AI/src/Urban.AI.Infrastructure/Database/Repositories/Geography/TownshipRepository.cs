namespace Urban.AI.Infrastructure.Database.Repositories.Geography;

#region Usings
using Urban.AI.Domain.Geography;
using Urban.AI.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
#endregion

internal sealed class TownshipRepository : Repository<Township>, ITownshipRepository
{
    public TownshipRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Township?> GetByTownshipDaneCodeAsync(
        string townshipDaneCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Townships
            .FirstOrDefaultAsync(t => t.TownshipDaneCode == townshipDaneCode, cancellationToken);
    }

    public async Task<IEnumerable<Township>> GetByMunicipalityDaneCodeAsync(
        string municipalityDaneCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Townships
            .Where(t => t.MunicipalityDaneCode == municipalityDaneCode)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }
}