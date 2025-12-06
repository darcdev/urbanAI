namespace Urban.AI.Infrastructure.Database.Repositories.Geography;

#region Usings
using Microsoft.EntityFrameworkCore;
using Urban.AI.Domain.Geography;
using Urban.AI.Infrastructure.Database;
#endregion

internal sealed class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Department?> GetByDepartmentDaneCodeAsync(
        string departmentDaneCode,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Departments
            .FirstOrDefaultAsync(d => d.DepartmentDaneCode == departmentDaneCode, cancellationToken);
    }
}