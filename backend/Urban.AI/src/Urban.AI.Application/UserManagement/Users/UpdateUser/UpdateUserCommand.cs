namespace Urban.AI.Application.UserManagement.Users.UpdateUser;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Users.Dtos;
#endregion

public class UpdateUserCommand : ICommand
{
    public Guid UserId { get; init; }
    public UpdateUserRequest UserToUpdate { get; init; }

    public UpdateUserCommand(Guid userId, UpdateUserRequest userToUpdate)
    {
        UserId = userId;
        UserToUpdate = userToUpdate;
    }
}
