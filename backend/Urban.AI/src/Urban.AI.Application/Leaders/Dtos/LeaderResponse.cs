namespace Urban.AI.Application.Leaders.Dtos;

public sealed record LeaderResponse(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    Guid DepartmentId,
    string DepartmentName,
    Guid MunicipalityId,
    string MunicipalityName,
    decimal Latitude,
    decimal Longitude,
    bool IsActive,
    DateTime CreatedAt);
