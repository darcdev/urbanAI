namespace Urban.AI.Domain.Geography;

using Urban.AI.Domain.Common.Abstractions;

public interface ITownshipRepository : IRepository<Township>
{
    Task<Township?> GetByTownshipDaneCodeAsync(string townshipDaneCode, CancellationToken cancellationToken = default);

    Task<IEnumerable<Township>> GetByMunicipalityDaneCodeAsync(string municipalityDaneCode, CancellationToken cancellationToken = default);
}