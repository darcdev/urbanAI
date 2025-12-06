namespace Urban.AI.Application.Geography.Common;

#region Usings
using Urban.AI.Domain.Geography;
#endregion

public interface IGeographyDataParser
{
    Task<IEnumerable<Department>> ParseDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Municipality>> ParseMunicipalitiesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Township>> ParseTownshipsAsync(CancellationToken cancellationToken = default);
}