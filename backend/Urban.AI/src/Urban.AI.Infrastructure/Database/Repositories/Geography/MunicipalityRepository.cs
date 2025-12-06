namespace Urban.AI.Infrastructure.Database.Repositories.Geography;

#region Usings
using Urban.AI.Domain.Geography;
using Urban.AI.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
#endregion

internal sealed class MunicipalityRepository : Repository<Municipality>, IMunicipalityRepository
{
    public MunicipalityRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Municipality?> GetByMunicipalityDaneCodeAsync(
        string municipalityDaneCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Municipalities
            .FirstOrDefaultAsync(m => m.MunicipalityDaneCode == municipalityDaneCode, cancellationToken);
    }

    public async Task<IEnumerable<Municipality>> GetByDepartmentDaneCodeAsync(
        string departmentDaneCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Municipalities
            .Where(m => m.DepartmentDaneCode == departmentDaneCode)
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }
}