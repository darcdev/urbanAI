namespace Urban.AI.Application.UserManagement.Roles.UpdateRole;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Roles.Dtos;
#endregion

public class UpdateRoleCommand : ICommand
{
    public string RoleName { get; init; }
    public UpdateRoleRequest RoleToUpdate { get; init; }

    public UpdateRoleCommand(string roleName, UpdateRoleRequest roleToUpdate)
    {
        RoleName = roleName;
        RoleToUpdate = roleToUpdate;
    }
}
