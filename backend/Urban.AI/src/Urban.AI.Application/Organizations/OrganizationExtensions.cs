namespace Urban.AI.Application.Organizations;

#region Usings
using Urban.AI.Application.Organizations.Dtos;
#endregion

public static class OrganizationExtensions
{
    public static OrganizationResponse ToOrganizationResponse(this Domain.Organizations.Organization organization)
    {
        return new OrganizationResponse(
            organization.Id,
            organization.UserId,
            organization.User.FirstName,
            organization.User.LastName,
            organization.User.Email,
            organization.OrganizationName,
            organization.IsActive,
            organization.CreatedAt);
    }
}
