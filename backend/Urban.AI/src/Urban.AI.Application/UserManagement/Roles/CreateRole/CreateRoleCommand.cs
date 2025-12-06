namespace Urban.AI.Application.UserManagement.Roles.CreateRole;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Roles.Dtos;
#endregion

public class CreateRoleCommand : ICommand
{
    public CreateRoleRequest RoleToCreate { get; init; }

    public CreateRoleCommand(CreateRoleRequest roleToCreate)
    {
        RoleToCreate = roleToCreate;
    }
}
