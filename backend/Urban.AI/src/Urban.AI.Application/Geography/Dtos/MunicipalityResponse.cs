namespace Urban.AI.Application.Geography.Dtos;

public record MunicipalityResponse(
    string MunicipalityDaneCode,
    string Name,
    string DepartmentDaneCode,
    decimal? Latitude = null,
    decimal? Longitude = null);