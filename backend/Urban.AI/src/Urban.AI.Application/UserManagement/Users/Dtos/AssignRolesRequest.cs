namespace Urban.AI.Application.UserManagement.Users.Dtos;

public record AssignRolesRequest(
    IEnumerable<string> RoleNames);
