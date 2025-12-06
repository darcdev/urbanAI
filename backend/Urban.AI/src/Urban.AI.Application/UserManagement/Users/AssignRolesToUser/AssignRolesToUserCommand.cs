namespace Urban.AI.Application.UserManagement.Users.AssignRolesToUser;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Users.Dtos;
#endregion

public class AssignRolesToUserCommand : ICommand
{
    public Guid UserId { get; init; }
    public AssignRolesRequest RolesToAssign { get; init; }

    public AssignRolesToUserCommand(Guid userId, AssignRolesRequest rolesToAssign)
    {
        UserId = userId;
        RolesToAssign = rolesToAssign;
    }
}
