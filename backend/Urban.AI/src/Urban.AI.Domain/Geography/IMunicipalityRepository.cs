namespace Urban.AI.Domain.Geography;

using Urban.AI.Domain.Common.Abstractions;

public interface IMunicipalityRepository : IRepository<Municipality>
{
    Task<Municipality?> GetByMunicipalityDaneCodeAsync(string municipalityDaneCode, CancellationToken cancellationToken = default);

    Task<IEnumerable<Municipality>> GetByDepartmentDaneCodeAsync(string departmentDaneCode, CancellationToken cancellationToken = default);
}