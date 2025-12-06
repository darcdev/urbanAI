namespace Urban.AI.Application.UserManagement.Users.DeleteUser;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
#endregion

public record DeleteUserCommand(Guid UserId) : ICommand;
