namespace Urban.AI.Application.Incidents.Dtos;

public sealed record IncidentResponse(
    Guid Id,
    string RadicateNumber,
    string ImageUrl,
    decimal Latitude,
    decimal Longitude,
    string? CitizenEmail,
    string? AdditionalComment,
    string? AiDescription,
    CategoryDto? Category,
    SubcategoryDto? Subcategory,
    LeaderDto? Leader,
    string Status,
    string Priority,
    DateTime CreatedAt);
