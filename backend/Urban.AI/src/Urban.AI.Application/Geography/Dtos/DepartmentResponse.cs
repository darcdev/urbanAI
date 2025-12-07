namespace Urban.AI.Application.Geography.Dtos;

public record DepartmentResponse(
    Guid Id,
    string DepartmentDaneCode,
    string Name,
    decimal? Latitude = null,
    decimal? Longitude = null);