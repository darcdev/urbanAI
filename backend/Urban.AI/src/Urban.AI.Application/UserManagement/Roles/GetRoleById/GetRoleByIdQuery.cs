namespace Urban.AI.Application.UserManagement.Roles.GetRoleById;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Roles.Dtos;
#endregion

public record GetRoleByIdQuery(string RoleName) : IQuery<RoleResponse>;
