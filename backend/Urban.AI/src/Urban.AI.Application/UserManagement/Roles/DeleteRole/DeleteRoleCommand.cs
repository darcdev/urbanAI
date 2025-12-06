namespace Urban.AI.Application.UserManagement.Roles.DeleteRole;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
#endregion

public record DeleteRoleCommand(string RoleName) : ICommand;
