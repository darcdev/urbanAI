namespace Urban.AI.Application.Organizations.Dtos;

public sealed record OrganizationResponse(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string OrganizationName,
    bool IsActive,
    DateTime CreatedAt);
