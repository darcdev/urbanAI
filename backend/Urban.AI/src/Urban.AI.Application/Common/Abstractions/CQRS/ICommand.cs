namespace Urban.AI.Application.Common.Abstractions.CQRS;

#region Usings
using MediatR;
using Urban.AI.Domain.Common.Abstractions; 
#endregion

public interface IBaseCommand
{

}

public interface ICommand : IRequest<Result>, IBaseCommand
{

}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{

}