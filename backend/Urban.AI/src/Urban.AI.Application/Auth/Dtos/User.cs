namespace Urban.AI.Application.Auth.Dtos;

public record User(
    Guid UserId,
    string Email,
    bool IsEmailVerified,
    DateTime CreatedAt,
    UserDetailsResponse? UserDetails);