namespace Urban.AI.Application.Auth;

#region Usings
using Urban.AI.Domain.Users;
using Urban.AI.Domain.Users.ValueObjects;
#endregion

internal static class UserExtensions
{
    public static Dtos.User ToDto(this User user)
    {
        return new Dtos.User(
            user.Id,
            user.Email,
            user.IsEmailVerified,
            user.CreatedAt,
            user.UserDetails?.ToDto()
        );
    }

    private static Dtos.UserDetailsResponse ToDto(this UserDetails userDetails)
    {
        return new Dtos.UserDetailsResponse(
            userDetails.PersonalInfo.ToDto(),
            userDetails.ContactInfo.ToDto()
        );
    }

    private static Dtos.PersonalInfoResponse ToDto(this PersonalInfo personalInfo)
    {
        return new Dtos.PersonalInfoResponse(
            personalInfo.BirthDate,
            personalInfo.Gender.Value,
            personalInfo.DocumentType.Value,
            personalInfo.DocumentNumber
        );
    }

    private static Dtos.ContactInfoResponse ToDto(this ContactInfo contactInfo)
    {
        return new Dtos.ContactInfoResponse(
            contactInfo.PhoneNumber.Value,
            contactInfo.PictureUrl,
            contactInfo.Biography
        );
    }
}
