namespace Urban.AI.Domain.Incidents;

using Urban.AI.Domain.Common.Abstractions;

public interface IIncidentRepository : IRepository<Incident>
{
    Task<Incident?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
    Task<Incident?> GetByRadicateNumberAsync(string radicateNumber, CancellationToken cancellationToken);
    Task<IEnumerable<Incident>> GetByLeaderIdAsync(Guid leaderId, CancellationToken cancellationToken);
    Task<IEnumerable<Incident>> GetByMunicipalityIdAsync(Guid municipalityId, CancellationToken cancellationToken);
    Task<IEnumerable<Incident>> GetByStatusAsync(IncidentStatus status, CancellationToken cancellationToken);
    Task<IEnumerable<Incident>> GetAllIncidentsAsync(CancellationToken cancellationToken);
}
