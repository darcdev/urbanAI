namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsEmailVerified,
    bool IsEnabled,
    DateTime CreatedAt,
    IReadOnlyCollection<string> Roles);