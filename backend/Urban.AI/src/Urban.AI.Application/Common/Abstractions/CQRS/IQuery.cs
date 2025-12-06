namespace Urban.AI.Application.Common.Abstractions.CQRS;

#region Usings
using MediatR;
using Urban.AI.Domain.Common.Abstractions;
#endregion

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{

}