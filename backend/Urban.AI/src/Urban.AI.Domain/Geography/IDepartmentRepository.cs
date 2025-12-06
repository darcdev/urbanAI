namespace Urban.AI.Domain.Geography;

using Urban.AI.Domain.Common.Abstractions;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department?> GetByDepartmentDaneCodeAsync(string departmentDaneCode, CancellationToken cancellationToken = default);
}