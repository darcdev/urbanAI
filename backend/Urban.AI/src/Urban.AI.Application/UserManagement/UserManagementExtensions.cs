namespace Urban.AI.Application.UserManagement;

#region Usings
using Urban.AI.Application.UserManagement.Users.Dtos;
using Urban.AI.Domain.Users;
using Urban.AI.Domain.Users.ValueObjects;
#endregion

internal static class UserManagementExtensions
{
    public static UserResponse ToUserResponse(this User user)
    {
        return new UserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsEmailVerified,
            user.IsEnabled,
            user.CreatedAt,
            [.. user.Roles.Select(r => r.Name)]
        );
    }

    public static PersonalInfo ToDomainPersonalInfo(this CompleteUserProfileRequest UserDetails)
    {
        return new PersonalInfo(
            UserDetails.PersonalInfo.BirthDate,
            Gender.Create(UserDetails.PersonalInfo.Gender),
            DocumentType.Create(UserDetails.PersonalInfo.DocumentType),
            UserDetails.PersonalInfo.DocumentNumber);
    }

    public static ContactInfo ToDomainContactInfo(this CompleteUserProfileRequest UserDetails)
    {
        return new ContactInfo(
            PhoneNumber.Create(UserDetails.ContactInfo.PhoneNumber),
            UserDetails.ContactInfo.PictureUrl,
            UserDetails.ContactInfo.Biography);
    }
}
