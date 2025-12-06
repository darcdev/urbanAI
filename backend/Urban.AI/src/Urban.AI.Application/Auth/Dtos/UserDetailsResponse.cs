namespace Urban.AI.Application.Auth.Dtos;

public record UserDetailsResponse(
    PersonalInfoResponse PersonalInfo,
    ContactInfoResponse ContactInfo);