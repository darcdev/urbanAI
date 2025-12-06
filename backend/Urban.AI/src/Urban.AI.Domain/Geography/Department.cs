namespace Urban.AI.Domain.Geography;

using Urban.AI.Domain.Common.Abstractions;

/// <summary>
/// Represents a Department (Estado/Departamento) entity in Colombia
/// </summary>
public sealed class Department : Entity
{
    public string DepartmentDaneCode { get; private set; }
    public string Name { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }

    private Department() { }

    private Department(
        string departmentDaneCode,
        string name,
        decimal? latitude,
        decimal? longitude) : base(Guid.NewGuid())
    {
        DepartmentDaneCode = departmentDaneCode;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Department Create(
        string departmentDaneCode,
        string name,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        return new Department(departmentDaneCode, name, latitude, longitude);
    }
}