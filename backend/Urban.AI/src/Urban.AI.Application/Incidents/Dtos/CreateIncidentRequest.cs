namespace Urban.AI.Application.Incidents.Dtos;

public sealed record CreateIncidentRequest(
    Stream ImageStream,
    string ImageFileName,
    string ImageContentType,
    decimal Latitude,
    decimal Longitude,
    string? CitizenEmail = null,
    string? AdditionalComment = null);
