namespace Urban.AI.Application.Auth.Register;

using Urban.AI.Application.Common.Abstractions.CQRS;

public record RegisterUserCommand(Dtos.RegisterRequest UserToRegister) : ICommand<Guid>;