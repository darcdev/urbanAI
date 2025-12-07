namespace Urban.AI.Application.Organizations.Dtos;

public sealed record CreateOrganizationRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string OrganizationName);
