namespace Urban.AI.Domain.Geography;

using Urban.AI.Domain.Common.Abstractions;

/// <summary>
/// Represents a Township (Corregimiento) entity in Colombia
/// </summary>
public sealed class Township : Entity
{
    public string TownshipDaneCode { get; private set; }
    public string Name { get; private set; }
    public string MunicipalityDaneCode { get; private set; }

    private Township() { }

    private Township(
        string townshipDaneCode,
        string name,
        string municipalityDaneCode) : base(Guid.NewGuid())
    {
        TownshipDaneCode = townshipDaneCode;
        Name = name;
        MunicipalityDaneCode = municipalityDaneCode;
    }

    public static Township Create(
        string townshipDaneCode,
        string name,
        string municipalityDaneCode)
    {
        return new Township(townshipDaneCode, name, municipalityDaneCode);
    }
}