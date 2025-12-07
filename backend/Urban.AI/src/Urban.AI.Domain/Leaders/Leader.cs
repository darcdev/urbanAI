namespace Urban.AI.Domain.Leaders;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
using Urban.AI.Domain.Leaders.Events;
using Urban.AI.Domain.Users;
#endregion

/// <summary>
/// Represents a Leader entity that manages reports in a specific geographic area
/// </summary>
public sealed class Leader : Entity
{
    #region Constants
    public const int MaxCoordinateDecimalPlaces = 8;
    #endregion

    private Leader(
        Guid userId,
        Guid departmentId,
        Guid municipalityId,
        decimal latitude,
        decimal longitude) : base(Guid.NewGuid())
    {
        UserId = userId;
        DepartmentId = departmentId;
        MunicipalityId = municipalityId;
        Latitude = latitude;
        Longitude = longitude;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private Leader() { }

    public Guid UserId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public Guid MunicipalityId { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; }
    public Department Department { get; private set; }
    public Municipality Municipality { get; private set; }

    public static Leader Create(
        Guid userId,
        Guid departmentId,
        Guid municipalityId,
        decimal latitude,
        decimal longitude)
    {
        var leader = new Leader(
            userId,
            departmentId,
            municipalityId,
            latitude,
            longitude);

        leader.AddDomainEvent(new LeaderCreatedDomainEvent(leader.Id));

        return leader;
    }

    public void UpdateCoordinates(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
