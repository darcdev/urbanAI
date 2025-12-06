namespace Urban.AI.Application.UserManagement.Users.GetUserById;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Users.Dtos;
#endregion

public record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
