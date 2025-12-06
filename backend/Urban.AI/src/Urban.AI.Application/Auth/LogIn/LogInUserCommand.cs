namespace Urban.AI.Application.Auth.LogIn;

using Urban.AI.Application.Common.Abstractions.CQRS;

public record LogInUserCommand(Dtos.LogInRequest UserToLogIn) : ICommand<Dtos.AccessTokenResponse>;