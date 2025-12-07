namespace Urban.AI.Application.Leaders;

#region Usings
using Urban.AI.Application.Leaders.Dtos;
using Urban.AI.Domain.Leaders;
#endregion

public static class LeaderExtensions
{
    public static LeaderResponse ToLeaderResponse(this Leader leader)
    {
        return new LeaderResponse(
            leader.Id,
            leader.UserId,
            leader.User.FirstName,
            leader.User.LastName,
            leader.User.Email,
            leader.DepartmentId,
            leader.Department.Name,
            leader.MunicipalityId,
            leader.Municipality.Name,
            leader.Latitude,
            leader.Longitude,
            leader.IsActive,
            leader.CreatedAt);
    }
}
