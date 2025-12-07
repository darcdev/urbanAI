namespace Urban.AI.Application.Incidents;

#region Usings
using Urban.AI.Application.Incidents.Dtos;
using Urban.AI.Domain.Incidents;
using Urban.AI.Domain.Leaders;
#endregion

internal static class IncidentExtensions
{
    public static IncidentResponse ToResponse(this Incident incident, string imageUrl)
    {
        return new IncidentResponse(
            incident.Id,
            incident.RadicateNumber,
            imageUrl,
            incident.Location.Latitude,
            incident.Location.Longitude,
            incident.CitizenEmail,
            incident.AdditionalComment,
            incident.AiDescription,
            incident.Category?.ToDto(),
            incident.Subcategory?.ToDto(),
            incident.Leader?.ToDto(),
            incident.Status.ToString(),
            incident.Priority.ToString(),
            incident.CreatedAt);
    }

    private static CategoryDto ToDto(this Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Code,
            category.Name);
    }

    private static SubcategoryDto ToDto(this Subcategory subcategory)
    {
        return new SubcategoryDto(
            subcategory.Id,
            subcategory.Name);
    }

    private static LeaderDto? ToDto(this Leader? leader)
    {
        if (leader?.User == null)
        {
            return null;
        }

        return new LeaderDto(
            leader.Id,
            $"{leader.User.FirstName} {leader.User.LastName}",
            leader.User.Email,
            leader.User.UserDetails?.ContactInfo.PhoneNumber.Value ?? string.Empty);
    }
}
