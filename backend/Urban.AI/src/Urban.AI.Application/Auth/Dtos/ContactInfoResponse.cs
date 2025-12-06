namespace Urban.AI.Application.Auth.Dtos;

public record ContactInfoResponse(
    string PhoneNumber,
    string? PictureUrl,
    string? Biography);