namespace Urban.AI.Domain.Geography;

using Urban.AI.Domain.Common.Abstractions;

/// <summary>
/// Represents a Municipality (Municipio) entity in Colombia
/// </summary>
public sealed class Municipality : Entity
{
    public string MunicipalityDaneCode { get; private set; }
    public string Name { get; private set; }
    public string DepartmentDaneCode { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }

    private Municipality() { }

    private Municipality(
        string municipalityDaneCode,
        string name,
        string departmentDaneCode,
        decimal? latitude,
        decimal? longitude) : base(Guid.NewGuid())
    {
        MunicipalityDaneCode = municipalityDaneCode;
        Name = name;
        DepartmentDaneCode = departmentDaneCode;
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Municipality Create(
        string municipalityDaneCode,
        string name,
        string departmentDaneCode,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        return new Municipality(municipalityDaneCode, name, departmentDaneCode, latitude, longitude);
    }
}