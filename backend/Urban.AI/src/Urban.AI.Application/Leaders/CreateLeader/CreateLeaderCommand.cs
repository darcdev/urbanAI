namespace Urban.AI.Application.Leaders.CreateLeader;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Leaders.Dtos;
#endregion

public sealed record CreateLeaderCommand(CreateLeaderRequest Request) : ICommand<Guid>;
