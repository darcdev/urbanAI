namespace Urban.AI.Application.UserManagement.Roles.GetRoles;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Roles.Dtos;
#endregion

public record GetRolesQuery : IQuery<IEnumerable<RoleResponse>>;
