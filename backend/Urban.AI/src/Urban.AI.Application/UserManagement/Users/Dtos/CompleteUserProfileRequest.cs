namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record CompleteUserProfileRequest(
    PersonalInfoRequest PersonalInfo,
    ContactInfoRequest ContactInfo);