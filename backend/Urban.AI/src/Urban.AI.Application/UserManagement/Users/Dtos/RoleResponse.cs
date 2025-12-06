namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record RoleResponse(
    string Id,
    string Name,
    string Description,
    bool Composite,
    bool ClientRole);