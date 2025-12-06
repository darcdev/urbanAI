namespace Urban.AI.Application.Geography.Dtos;

public record DepartmentResponse(
    string DepartmentDaneCode,
    string Name,
    decimal? Latitude = null,
    decimal? Longitude = null);