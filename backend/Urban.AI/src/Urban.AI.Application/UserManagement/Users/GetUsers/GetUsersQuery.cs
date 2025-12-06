namespace Urban.AI.Application.UserManagement.Users.GetUsers;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Users.Dtos;
#endregion

public record GetUsersQuery : IQuery<IEnumerable<UserResponse>>;
