namespace Urban.AI.Application.Common.Abstractions.CQRS;

#region Usings
using MediatR;
using Urban.AI.Domain.Common.Abstractions;
#endregion

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{

}