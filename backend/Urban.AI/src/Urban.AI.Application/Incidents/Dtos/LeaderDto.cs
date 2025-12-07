namespace Urban.AI.Application.Incidents.Dtos;

public sealed record LeaderDto(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber);
