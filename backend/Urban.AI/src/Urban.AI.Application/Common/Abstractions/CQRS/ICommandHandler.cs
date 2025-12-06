namespace Urban.AI.Application.Common.Abstractions.CQRS;

#region Usings
using MediatR;
using Urban.AI.Domain.Common.Abstractions; 
#endregion

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}