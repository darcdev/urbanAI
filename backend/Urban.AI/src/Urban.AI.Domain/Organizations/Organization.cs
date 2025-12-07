namespace Urban.AI.Domain.Organizations;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Organizations.Events;
using Urban.AI.Domain.Users;
#endregion

/// <summary>
/// Represents an Organization that can view statistical reports in the system
/// </summary>
public sealed class Organization : Common.Abstractions.Entity
{
    #region Constants
    public const int MaxOrganizationNameLength = 200;
    #endregion

    private Organization(
        Guid userId,
        string organizationName) : base(Guid.NewGuid())
    {
        UserId = userId;
        OrganizationName = organizationName;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    private Organization() { }

    public Guid UserId { get; private set; }
    public string OrganizationName { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; }

    public static Organization Create(
        Guid userId,
        string organizationName)
    {
        var organization = new Organization(
            userId,
            organizationName);

        organization.AddDomainEvent(new OrganizationCreatedDomainEvent(organization.Id));

        return organization;
    }

    public void UpdateOrganizationName(string organizationName)
    {
        OrganizationName = organizationName;
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
