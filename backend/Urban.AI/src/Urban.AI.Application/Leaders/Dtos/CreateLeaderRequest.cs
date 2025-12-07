namespace Urban.AI.Application.Leaders.Dtos;

public sealed record CreateLeaderRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    Guid DepartmentId,
    Guid MunicipalityId,
    decimal Latitude,
    decimal Longitude);
