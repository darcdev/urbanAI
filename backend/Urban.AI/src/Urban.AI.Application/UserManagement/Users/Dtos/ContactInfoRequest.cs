namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record ContactInfoRequest(
    string PhoneNumber,
    string? PictureUrl,
    string? Biography);
