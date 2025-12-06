namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record UserKeycloakInfo(
    string IdentityId,
    string Email,
    string FirstName,
    string LastName,
    bool EmailVerified,
    bool Enabled);
