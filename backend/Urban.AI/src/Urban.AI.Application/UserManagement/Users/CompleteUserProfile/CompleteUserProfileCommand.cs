namespace Urban.AI.Application.UserManagement.Users.CompleteUserProfile;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Users.Dtos;
#endregion

public record CompleteUserProfileCommand(CompleteUserProfileRequest UserDetails) : ICommand;