namespace Urban.AI.Application.Geography.Dtos;

public record TownshipResponse(
    Guid Id,
    string TownshipDaneCode,
    string Name,
    string MunicipalityDaneCode);